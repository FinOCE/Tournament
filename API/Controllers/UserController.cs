namespace API.Controllers;

public class UserController : Controller
{
    private readonly IConfiguration _Configuration;
    private readonly SnowflakeService _SnowflakeService;
    private readonly DbService _DbService;
    private readonly LoggingService _LoggingService;
    private readonly CaptchaService _CaptchaService;

    public UserController(
        IConfiguration configuration,
        SnowflakeService snowflakeService,
        DbService dbService,
        LoggingService loggingService,
        CaptchaService captchaService)
    {
        _Configuration = configuration;
        _SnowflakeService = snowflakeService;
        _DbService = dbService;
        _LoggingService = loggingService;
        _CaptchaService = captchaService;
    }

    [Route("users/{id:snowflake}")]
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get a user by their snowflake ID",
        Description = "Returns the requested user")]
    [SwaggerResponse(200, "Returns the requested user", typeof(User), new[] { "application/json" })]
    [SwaggerResponse(404, "The given ID did not match a user")]
    public async Task<IActionResult> Get(
        [SwaggerParameter(
            Required = true,
            Description = "The snowflake ID of the user to be fetched")]
        string id)
    {
        try
        {
            try
            {
                // Attempt to fetch the user from the database
                User user = (await _DbService
                    .RunProcedure(
                        "[dbo].[tsp_GetUser]",
                        new Dictionary<string, object> { { "@Id", id } },
                        reader => new User(
                            (string)reader["Id"],
                            (string)reader["Username"],
                            (int)reader["Discriminator"],
                            (string)reader["Icon"],
                            (int)reader["Verified"] == 1,
                            permissions: (int)reader["Permissions"])))
                    .First();

                // Return existing user
                return Ok(user);
            }
            catch (InvalidOperationException)
            {
                // Return 404 if user doesn't exist
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _LoggingService.Log(nameof(UserController), ex.Message);
            return Problem("An unknown error occurred attempting to get the user");
        }
    }

    public struct UserPostBody
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }

    [Route("users")]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new user",
        Description = "Returns the newly created user")]
    [SwaggerResponse(200, "Returns the created user", typeof(User), new[] { "application/json" })]
    [SwaggerResponse(400, "The given body was not valid for the provided reason")]
    public async Task<IActionResult> Post(
        [FromBody] UserPostBody body)
    {
        // TODO: Ensure all body non-null body properties exist
        // TODO: Check that the email is valid
        // TODO: Filter username for bad words

        try
        {
            // Check if the captcha was successfully completed
            bool captchaSuccess = await _CaptchaService.Validate(body.Token);
            if (!captchaSuccess)
                return BadRequest("The captcha could not be validated");

            // Check if email is already in user
            bool exists = (await _DbService
                .RunProcedure(
                    "[dbo].[tsp_CheckEmail]",
                    new Dictionary<string, object> { { "@Email", body.Email } },
                    reader => (int)reader["Exists"] != 0))
                .Any(r => !r);

            if (exists)
                return BadRequest("Email is already in use");

            // Choose a random discriminator for the user
            int[] unavailableDiscriminators = await _DbService
                .RunProcedure(
                    "[dbo].[tsp_FindExistingDiscriminators]",
                    new Dictionary<string, object> { { "@Username", body.Username } },
                    reader => (int)reader["Discriminator"]);

            if (unavailableDiscriminators.Length == 10000)
                return BadRequest("No available discriminators for that username");

            int[] availableDiscriminators = (new int[10000])
                .Select((v, i) => i)
                .Where(i => !unavailableDiscriminators.Any(v => v == i))
                .ToArray();

            Random random = new();
            int discriminator = availableDiscriminators[random.Next(availableDiscriminators.Length)];

            // Hash the password
            if (_Configuration["PASSWORD_HASHING_SECRET"] is null)
                throw new ApplicationException("Password hashing secret env variable not set");

            string hashedPassword;

            using (HMACSHA256 hash = new(Encoding.UTF8.GetBytes(_Configuration["PASSWORD_HASHING_SECRET"])))
            {
                byte[] passwordBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(body.Password));
                hashedPassword = Encoding.UTF8.GetString(passwordBytes);
            }

            // Create user
            User user = (await _DbService
                .RunProcedure(
                    "[dbo].[tsp_CreateUser]",
                    new Dictionary<string, object>
                    {
                        { "@Id", _SnowflakeService.Generate().ToString() },
                        { "@Email", body.Email },
                        { "@Username", body.Username },
                        { "@Discriminator", discriminator },
                        { "@Password", hashedPassword }
                    },
                    reader => new User(
                        (string)reader["Id"],
                        (string)reader["Username"],
                        (int)reader["Discriminator"],
                        (string)reader["Icon"],
                        (int)reader["Verified"] == 1,
                        permissions: (int)reader["Permissions"])))
                .First();

            return Created($"/users/{user.Id}", user); // TODO: Use proper URL with complete path
        }
        catch (Exception ex)
        {
            _LoggingService.Log(nameof(UserController), ex.Message);
            return Problem("An unknown error occurred attempting to create the user");
        }
    }

    public struct UserPatchBody
    {
        public string? Email;
        public string? Username;
        public string? Password;
        public string? Discriminator;
        public string? Icon;
        public int? Permissions;
        public bool? Verified;
    }

    [Route("users/{id:snowflake}")]
    [HttpPatch]
    [SwaggerOperation(
        Summary = "Update a user's details by their snowflake ID",
        Description = "Returns the user after their details were updated")]
    [SwaggerResponse(200, "Returns the updated user", typeof(User), new[] { "application/json" })]
    [SwaggerResponse(404, "The given ID did not match a user")]
    public async Task<IActionResult> Patch(
        string id,
        [FromBody] UserPatchBody body)
    {
        // TODO: Add additional validation for special changes (permissions, verified)
        
        try
        {
            // Add new values to procedure where provided
            Dictionary<string, object> parameters = new()
            {
                { "@Id", id }
            };
        
            if (body.Email is not null)
                parameters.Add("@Email", body.Email);
            if (body.Username is not null)
                parameters.Add("@Username", body.Username);
            if (body.Password is not null)
                parameters.Add("@Password", body.Password);
            if (body.Discriminator is not null)
                parameters.Add("@Discriminator", body.Discriminator);
            if (body.Icon is not null)
                parameters.Add("@Icon", body.Icon);
            if (body.Permissions is not null)
                parameters.Add("@Permissions", body.Permissions);
            if (body.Verified is not null)
                parameters.Add("@Verified", (bool)body.Verified ? 1 : 0);
            
            // Update user
            User user = (await _DbService
                .RunProcedure(
                    "[dbo].[tsp_UpdateUser]",
                    parameters,
                    reader => new User(
                        (string)reader["Id"],
                        (string)reader["Username"],
                        (int)reader["Discriminator"],
                        (string)reader["Icon"],
                        (int)reader["Verified"] == 1,
                        permissions: (int)reader["Permissions"])))
                .First();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _LoggingService.Log(nameof(UserController), ex.Message);
            return Problem("An unknown error occurred attempting to update the user");
        }
    }
}
