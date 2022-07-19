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
                            (reader.IsDBNull("Icon") ? null : (string)reader["Icon"]),
                            (bool)reader["Verified"],
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
    [SwaggerResponse(201, "Returns the created user", typeof(User), new[] { "application/json" })]
    [SwaggerResponse(400, "The given body was not valid for the provided reason")]
    public async Task<IActionResult> Post(
        [FromBody] UserPostBody body)
    {
        // Check if the captcha was successfully completed
        bool captchaSuccess = await _CaptchaService.Validate(body.Token);
        if (!captchaSuccess)
            return BadRequest("The captcha could not be validated");

        // Check that all essential contents of the body exists
        if (body.Email is null)
            return BadRequest($"Missing {body.Email} in body");

        if (body.Username is null)
            return BadRequest($"Missing {body.Username} in body");

        if (body.Password is null)
            return BadRequest($"Missing {body.Password} in body");

        // Validate email address
        try
        {
            _ = new MailAddress(body.Email);
        }
        catch (Exception)
        {
            return BadRequest("Invalid email address provided");
        }

        // Validate username
        try
        {
            _ = new User("1", body.Username, 1);
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid username provided");
        }

        // Validate password
        if (body.Password.Length < 8)
            return BadRequest("Password must be at least 8 characters long");

        try
        {
            // Check if email is already in user
            bool exists = (await _DbService
                .RunProcedure(
                    "[dbo].[tsp_CheckEmail]",
                    new Dictionary<string, object> { { "@Email", body.Email } },
                    reader => (bool)reader["Exists"]))
                .Any(r => r);

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

            using HMACSHA256 hash = new(Encoding.UTF8.GetBytes(_Configuration["PASSWORD_HASHING_SECRET"]));
            byte[] passwordBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(body.Password));
            hashedPassword = Convert.ToBase64String(passwordBytes);

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
                        (reader.IsDBNull("Icon") ? null : (string)reader["Icon"]),
                        (bool)reader["Verified"],
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
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int? Discriminator { get; set; }
        public string? Icon { get; set; }
        public int? Permissions { get; set; }
        public bool? Verified { get; set; }
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
        // TODO: Setup icon upload

        Dictionary<string, object> parameters = new()
        {
            { "@Id", id }
        };

        User dummy = API.Models.Users.User.Dummy();

        // Validate email address (if present)
        if (body.Email is not null)
        {
            try
            {
                _ = new MailAddress(body.Email);
                parameters.Add("@Email", body.Email);
            }
            catch (Exception)
            {
                return BadRequest("Invalid email address provided");
            }
        }

        // Validate username (if present)
        if (body.Username is string @username)
            if (!dummy.SetUsername(@username))
                return BadRequest("Invalid username provided");
            else
                parameters.Add("@Username", body.Username);

        // Validate and hash password (if present)        
        if (body.Password is string @passsword)
        {
            if (@passsword.Length < 8)
                return BadRequest("Password must be at least 8 characters long");

            string? hashedPassword = null;
            
            if (_Configuration["PASSWORD_HASHING_SECRET"] is null)
                throw new ApplicationException("Password hashing secret env variable not set");

            using HMACSHA256 hash = new(Encoding.UTF8.GetBytes(_Configuration["PASSWORD_HASHING_SECRET"]));
            byte[] passwordBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(body.Password));
            hashedPassword = Convert.ToBase64String(passwordBytes);

            parameters.Add("@Password", hashedPassword);
        }

        // Validate discriminator (if present)
        if (body.Discriminator is int @discriminator)
            if (!dummy.SetDiscriminator(@discriminator))
                return BadRequest("Invalid discriminator provided");
            else
                parameters.Add("@Discriminator", body.Discriminator);

        // Validate icon (if present)
        if (body.Icon is null || body.Icon is string)
            if (!dummy.SetIcon(body.Icon))
                return BadRequest("Invalid icon provided");
            else
                parameters.Add("@Icon", body.Icon is null ? DBNull.Value : body.Icon);

        // Validate permissions (if present)
        if (body.Permissions is int @permissions)
        {
            // TODO: Check the request is made by someone with permission to set permissions
            
            if (body.Permissions < 0)
                return BadRequest("Permissions must be at least 0");
            else
                parameters.Add("@Permissions", body.Permissions);
        }

        // Validate verified (if present)
        if (body.Verified is not null)
        {
            // TODO: Check the request is made by someone with permission to set verified

            if (body.Verified is not bool)
                return BadRequest("Invalid verified provided");
            else
                parameters.Add("@Verified", (bool)body.Verified ? 1 : 0);
        }
        
        try
        {
            // Update user
            User user = (await _DbService
                .RunProcedure(
                    "[dbo].[tsp_UpdateUser]",
                    parameters,
                    reader => new User(
                        (string)reader["Id"],
                        (string)reader["Username"],
                        (int)reader["Discriminator"],
                        (reader.IsDBNull("Icon") ? null : (string)reader["Icon"]),
                        (bool)reader["Verified"],
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
