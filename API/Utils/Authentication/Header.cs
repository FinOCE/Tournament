namespace API.Utils.Authentication;

/// <summary>
/// The header of a JWT
/// </summary>
public class Header
{
    public string Type = "JWT";
    public string Algorithm = "HS256";

    /// <summary>
    /// Get the token header as a base64 encoded string
    /// </summary>
    /// <returns>The header formatted for a JWT</returns>
    public override string ToString()
    {
        string json = $@"
            {{
                ""typ"": ""{Type}"",
                ""alg"": ""{Algorithm}""
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
