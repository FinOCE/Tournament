namespace API.Models.Socials;

/// <summary>
/// A social media reference
/// </summary>
public class Social
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Icon { get; init; }
    public string Link { get; private set; }

    public Social(string id, SocialPlatform platform, string link)
    {
        // Validate
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (!Enum.IsDefined(platform))
            throw new ArgumentException($"Invalid {nameof(platform)} provided");

        if (!SetLink(link))
            throw new ArgumentException($"Invalid {nameof(link)} provided");

        // Instantiate
        Id = id;
        Name = Enum.GetName(platform)!;
        Icon = GetPlatformIcon(platform);
        Link = link;
    }

    /// <summary>
    /// Set the link to the social media account
    /// </summary>
    public bool SetLink(string link)
    {
        // Regex slightly modified from https://urlregex.com/
        return new StringValidator()
            .SetMinimumLength(1)
            .SetInvalidRegex(new(@"^http(s ?)\:\/\/[0-9a-zA-Z]([-.\w] *[0-9a-zA-Z]) * (: (0-9) *) * (\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$"))
            .OnSuccess(link => Link = link!)
            .Test(link);
    }

    /// <summary>
    /// Get the icon of a platform
    /// </summary>
    public static string GetPlatformIcon(SocialPlatform platform)
    {
        return platform switch // TODO: Create platform icon paths
        {
            SocialPlatform.Discord => "1234567890123456",
            SocialPlatform.Facebook => "1234567890123456",
            SocialPlatform.Instagram => "1234567890123456",
            SocialPlatform.Tiktok => "1234567890123456",
            SocialPlatform.Twitch => "1234567890123456",
            SocialPlatform.Twitter => "1234567890123456",
            SocialPlatform.Website => "1234567890123456",
            SocialPlatform.YouTube => "1234567890123456",
            _ => "1234567890123456"
        };
    }
}

public enum SocialPlatform
{
    Discord,
    Facebook,
    Instagram,
    Tiktok,
    Twitch,
    Twitter,
    Website,
    YouTube
}
