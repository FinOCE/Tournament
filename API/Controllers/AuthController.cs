namespace API.Controllers;

public class AuthController : Controller
{
    private readonly IConfiguration _Configuration;
    private readonly SnowflakeService _SnowflakeService;
    private readonly DbService _DbService;
    private readonly LoggingService _LoggingService;
    private readonly CaptchaService _CaptchaService;

    public AuthController(
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

    public struct AuthPostBody
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }

    [Route("login")]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Authenticate a user",
        Description = "Returns the JWT for a user account that can be used for authorizing later requests")]
    [SwaggerResponse(200, "Returns the JWT of the authenticated user", typeof(string), new[] { "text/plain" })]
    [SwaggerResponse(400, "The given body was not valid for the provided reason")]
    [SwaggerResponse(404, "Provided details didn't match a user")]
    public async Task<IActionResult> Post(
        [FromBody] AuthPostBody body)
    {
        // Check if the captcha was successfully completed
        bool captchaSuccess = await _CaptchaService.Validate(body.Token);
        if (!captchaSuccess)
            return BadRequest("The captcha could not be validated");

        // Check that all essential contents of the body exists
        if (body.Email is null)
            return BadRequest($"Missing {body.Email} in body");

        if (body.Password is null)
            return BadRequest($"Missing {body.Password} in body");

        // Validate the email address
        try
        {
            _ = new MailAddress(body.Email);
        }
        catch (Exception)
        {
            return BadRequest("Invalid email address provided");
        }

        // Validate password
        if (body.Password.Length < 8)
            return BadRequest("Password must be at least 8 characters long");

        // Hash password
        using HMACSHA256 hash = new(Encoding.UTF8.GetBytes(_Configuration["PASSWORD_HASHING_SECRET"]));
        byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(body.Password));
        string hashedPassword = Convert.ToBase64String(bytes);
        
        try
        {
            // Attempt to fetch user from database
            User? user = null;
            
            try
            {
                user = (await _DbService
                    .RunProcedure(
                        "[dbo].[tsp_Login]",
                        new Dictionary<string, object>
                        {
                            { "@Email", body.Email },
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

                if (user == null)
                    throw new Exception();
            }
            catch (Exception)
            {
                return NotFound("Invalid email or password provided");
            }

            // Generate JWT from user
            Snowflake tokenSnowflake = _SnowflakeService.Generate();
            Token token = new(
                tokenSnowflake.ToString(),
                subject: user.Id,
                expiration: tokenSnowflake.Timestamp.AddDays(7),
                notBefore: tokenSnowflake.Timestamp,
                issuedAt: tokenSnowflake.Timestamp);

            // Save login to history
            _ = await _DbService.RunProcedure(
                "[dbo].[tsp_SaveToken]",
                new Dictionary<string, object>
                {
                    { "@TokenId", token.Payload.Id },
                    { "@UserId", user.Id }
                },
                reader => 0);

            // Return the JWT
            return Ok(token.ToString());
        }
        catch (Exception ex)
        {
            _LoggingService.Log(nameof(UserController), ex.Message);
            return Problem("An unknown error occurred attempting to authenticate");
        }

        /*
         * TODO: Create tsp_Login to handle logging into an account
         * TODO: Write tests for tsp_Login stored procedure
         * TODO: Create tsp_SaveToken to add a token to the database
         * TODO: Write tests for tsp_SaveToken stored procedure
         */
    }
}
