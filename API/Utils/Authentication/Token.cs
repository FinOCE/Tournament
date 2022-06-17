namespace API.Utils.Authentication;

/// <summary>
/// A JSON Web Token (JWT) as described by <see href="https://datatracker.ietf.org/doc/html/rfc7519">RFC 7519</see>
/// </summary>
public class Token
{
    public const int DaysAlive = 7;

    public Header Header { get; init; }
    public Payload Payload { get; init; }

    /// <summary>
    /// Instantiate a token
    /// </summary>
    /// <param name="id">The token ID</param>
    /// <param name="issuer">The principal that issued the ID</param>
    /// <param name="subject">The principal that is the subject of the token</param>
    /// <param name="audience">The recipiant the token is intended for</param>
    /// <param name="expiration">The time on or after that the token should not be accepted</param>
    /// <param name="notBefore">The time before the token should be accepted</param>
    /// <param name="issuedAt">The time the token is issued</param>
    /// <exception cref="ArgumentException"></exception>
    public Token(
        string id,
        string? issuer = null,
        string? subject = null,
        string? audience = null,
        DateTime? expiration = null,
        DateTime? notBefore = null,
        DateTime? issuedAt = null)
    {
        // Validate
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        // Instantiate
        Header = new();
        Payload = new(
            id,
            issuer,
            subject,
            audience,
            expiration,
            notBefore,
            issuedAt);
    }

    /// <summary>
    /// Get the token formatted as a string
    /// </summary>
    /// <returns>The token in string format</returns>
    /// <exception cref="ApplicationException"></exception>
    public override string ToString()
    {
        // Generate the signature
        string signature;

        string? secret = Environment.GetEnvironmentVariable("AUTHORIZATION_SECRET");
        if (secret is null)
            throw new ApplicationException("Authorization secret env variable not set");

        byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

        using (HMACSHA256 hash = new(secretBytes))
        {
            string encoded = Header.ToString() + '.' + Payload.ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(encoded);

            byte[] signatureBytes = hash.ComputeHash(bytes);
            signature = Convert.ToBase64String(signatureBytes);
        }

        // Return with formed JWT
        return Header.ToString() + '.' + Payload.ToString() + '.' + signature;
    }

    /// <summary>
    /// Validate a token
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>The token deserialized into a class</returns>
    /// <exception cref="ApplicationException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static Token Deserialize(string jwt)
    {
        // Ensure the token is 3 parts split by dots
        if (jwt.Split('.').Length != 3)
            throw new ArgumentException($"Invalid {nameof(jwt)} provided");

        // Validate the signature
        string? secret = Environment.GetEnvironmentVariable("AUTHORIZATION_SECRET");
        if (secret is null)
            throw new ApplicationException("Authorization secret env variable not set");

        byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

        using (HMACSHA256 hash = new(secretBytes))
        {
            string encoded = string.Join('.', jwt.Split('.')[..2]);
            byte[] bytes = Encoding.UTF8.GetBytes(encoded);

            byte[] signatureBytes = hash.ComputeHash(bytes);

            if (jwt.Split('.')[2] != Convert.ToBase64String(signatureBytes))
                throw new ArgumentException("Token payload did not match signature");
        }

        // Attempt to deserialize the token
        Token token;
        try
        {
            JsonDocument payload = JsonDocument.Parse(
                Encoding.UTF8.GetString(
                    Convert.FromBase64String(jwt.Split('.')[1])));

            // Require an ID
            JsonValueKind id = payload.RootElement.GetProperty("jti").ValueKind;
            if (id == JsonValueKind.Undefined)
                throw new Exception();

            // Functions to handle getting different types of properties
            Func<string, string?> getString = (string property) =>
                payload.RootElement.TryGetProperty(property, out JsonElement value)
                    ? value.GetString()
                    : null;

            Func<string, DateTime?> getDateTime = (string property) =>
                payload.RootElement.TryGetProperty(property, out JsonElement value)
                    ? new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(value.GetInt64())
                    : null;

            // Generate token from payload
            token = new(
                getString("jti")!,
                getString("iss"),
                getString("sub"),
                getString("aud"),
                getDateTime("exp"),
                getDateTime("nbf"),
                getDateTime("iat"));
        }
        catch
        {
            throw new ArgumentException($"Invalid {nameof(jwt)} provided");
        }

        // Return the deserialized and validated token
        return token;
    }
}
