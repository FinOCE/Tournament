namespace API.Controllers;

public class UserController : Controller
{
    private readonly string? _PasswordSecret = Environment.GetEnvironmentVariable("PASSWORD_HASHING_SECRET");

    private readonly SnowflakeService _SnowflakeService;
    private readonly DbService _DbService;
    private readonly LoggingService _LoggingService;
    private readonly CaptchaService _CaptchaService;

    public UserController(
        SnowflakeService snowflakeService,
        DbService dbService,
        LoggingService loggingService,
        CaptchaService captchaService)
    {
        _SnowflakeService = snowflakeService;
        _DbService = dbService;
        _LoggingService = loggingService;
        _CaptchaService = captchaService;
    }

    [Route("users")]
    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] string email,
        [FromBody] string username,
        [FromBody] string password,
        [FromBody] string token)
    {
        try
        {
            // Check if the captcha was successfully completed
            bool captchaSuccess = await _CaptchaService.Validate(token);
            if (!captchaSuccess)
                return BadRequest("The captcha could not be validated");

            // Check if email is already in user
            bool exists = _DbService
                .RunProcedure(
                    "[dbo].[tsp_CheckEmail]",
                    new Dictionary<string, object> { { "@Email", email } },
                    reader => (int)reader["Exists"] != 0)
                .Any(r => !r);

            if (exists)
                return BadRequest("Email is already in use");

            // Choose a random discriminator for the user
            int[] unavailableDiscriminators = _DbService
                .RunProcedure(
                    "[dbo].[tsp_FindExistingDiscriminators]",
                    new Dictionary<string, object> { { "@Username", username } },
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
            if (_PasswordSecret is null)
                throw new ApplicationException("Password hashing secret env variable not set");

            using (HMACSHA256 hash = new(Encoding.UTF8.GetBytes(_PasswordSecret)))
            {
                byte[] passwordBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                password = Encoding.UTF8.GetString(passwordBytes);
            }

            // Create user
            User user = _DbService
                .RunProcedure(
                    "[dbo].[tsp_CreateUser]",
                    new Dictionary<string, object>
                    {
                        { "@Id", _SnowflakeService.Generate().ToString() },
                        { "@Email", email },
                        { "@Username", username },
                        { "@Discriminator", discriminator },
                        { "@Password", password }
                    },
                    reader => new User(
                        (string)reader["Id"],
                        (string)reader["Username"],
                        (int)reader["Discriminator"],
                        (string)reader["Icon"],
                        (int)reader["Verified"] == 1))
                .First();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _LoggingService.Log(nameof(UserController), ex.Message);
            return Problem("An unknown error occurred attempting to create the account");
        }
    }
}
