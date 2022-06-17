namespace API.Utils.Authentication;

/// <summary>
/// The payload of a JWT
/// </summary>
public class Payload
{
    public string Id { get; init; }
    public string? Issuer { get; init; }
    public string? Subject { get; init; }
    public string? Audience { get; init; }
    public DateTime? Expiration { get; protected set; }
    public DateTime? NotBefore { get; protected set; }
    public DateTime? IssuedAt { get; protected set; }

    /// <summary>
    /// Instantiate a JWT payload
    /// </summary>
    /// <param name="id">The token ID</param>
    /// <param name="issuer">The principal that issued the ID</param>
    /// <param name="subject">The principal that is the subject of the token</param>
    /// <param name="audience">The recipiant the token is intended for</param>
    /// <param name="expiration">The time on or after that the token should not be accepted</param>
    /// <param name="notBefore">The time before the token should be accepted</param>
    /// <param name="issuedAt">The time the token is issued</param>
    /// <exception cref="ArgumentException"></exception>
    public Payload(
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
        Id = id;
        Issuer = issuer;
        Subject = subject;
        Audience = audience;
        Expiration = expiration;
        NotBefore = notBefore;
        IssuedAt = issuedAt;
    }

    /// <summary>
    /// Set the expiration time of the token
    /// </summary>
    /// <param name="time">The time the token should expire</param>
    public void SetExpiration(DateTime time)
    {
        Expiration = time;
    }

    /// <summary>
    /// Set the time the token cannot be used until
    /// </summary>
    /// <param name="time">The time the token cannot be used until</param>
    public void SetNotBefore(DateTime time)
    {
        NotBefore = time;
    }

    /// <summary>
    /// Set the time the token is issued
    /// </summary>
    /// <param name="time">The time the token is issued</param>
    public void SetIssuedAt(DateTime time)
    {
        IssuedAt = time;
    }

    /// <summary>
    /// Get the token payload as a base64 encoded string
    /// </summary>
    /// <returns>The payload formatted for a JWT</returns>
    public override string ToString()
    {
        string json = $@"
            {{
                {(Issuer is not null ? $"\"iss\": \"{Issuer}\"," : "")}
                {(Subject is not null ? $"\"sub\": \"{Subject}\"," : "")}
                {(Audience is not null ? $"\"aud\": \"{Audience}\"," : "")}
                {(Expiration is not null ? $"\"exp\": {((DateTimeOffset)Expiration).ToUnixTimeSeconds()}," : "")}
                {(NotBefore is not null ? $"\"nbf\": {((DateTimeOffset)NotBefore).ToUnixTimeSeconds()}," : "")}
                {(IssuedAt is not null ? $"\"iat\": {((DateTimeOffset)IssuedAt).ToUnixTimeSeconds()}," : "")}
                ""jti"": ""{Id}""
            }}
        ";

        byte[] bytes = Encoding.UTF8.GetBytes(
            Regex.Replace(
                json,
                @"\s+",
                " "));

        return Convert.ToBase64String(bytes);
    }
}
