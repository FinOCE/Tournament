namespace API.Models.Events;

/// <summary>
/// A tournament that can include events
/// </summary>
public class Tournament
{
    public const string DefaultIcon = "1234567890123456"; // TODO: Create default icon path
    public const string DefaultBanner = "1234567890123456"; // TODO: Create default banner path

    public string Id { get; init; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Rules { get; private set; }
    public Dictionary<string, Coordinator> Coordinators { get; init; }
    public string Icon { get; private set; }
    public string Banner { get; private set; }
    public Game? Game { get; private set; }
    public Dictionary<string, Social> Socials { get; init; }
    public bool Verified { get; private set; }
    // TODO: Include a dictionary of events (new class)
    // TODO: Include a section for prizes

    /// <exception cref="ArgumentException"></exception>
    public Tournament(
        string id,
        string name,
        string? description = null,
        string? rules = null,
        Dictionary<string, Coordinator>? coordinators = null,
        string? icon = DefaultIcon,
        string? banner = DefaultBanner,
        Game? game = null,
        Dictionary<string, Social>? socials = null,
        bool verified = false)
    {
        // Validate arguments
        if (!SetName(name))
            throw new ArgumentException($"Invalid {nameof(name)} provided");

        if (!SetDescription(description))
            throw new ArgumentException($"Invalid {nameof(description)} provided");

        if (!SetRules(rules))
            throw new ArgumentException($"Invalid {nameof(rules)} provided");

        if (!SetIcon(icon))
            throw new ArgumentException($"Invalid {nameof(icon)} provided");

        if (!SetBanner(banner))
            throw new ArgumentException($"Invalid {nameof(banner)} provided");

        // Assign arguments to tournament
        Id = id;
        Name = name;
        Description = description;
        Rules = rules;
        Icon = icon ?? DefaultIcon;
        Banner = banner ?? DefaultBanner;
        Coordinators = coordinators ?? new();
        Game = game;
        Socials = socials ?? new();
        Verified = verified;
    }

    /// <summary>
    /// Set the name of the tournament
    /// </summary>
    public bool SetName(string name)
    {
        return new StringValidator()
            .Trim()
            .SetMinimumLength(3)
            .SetMaximumLength(32)
            .SetInvalidRegex(new(@"[^\w-. ]"))
            .OnSuccess(name => Name = name!)
            .Test(name);
    }

    /// <summary>
    /// Set the description of the tournament
    /// </summary>
    public bool SetDescription(string? description)
    {
        return new StringValidator()
            .AllowNull()
            .Trim()
            .SetMaximumLength(2048)
            .SetInvalidRegex(new(@"[^\w-. ]"))
            .OnSuccess(description => Description = description)
            .Test(description);
    }

    /// <summary>
    /// Set the rules of the tournament
    /// </summary>
    public bool SetRules(string? rules)
    {
        return new StringValidator()
            .AllowNull()
            .Trim()
            .SetMaximumLength(2048)
            .SetInvalidRegex(new(@"[^\w-. ]"))
            .OnSuccess(rules => Rules = rules)
            .Test(rules);
    }

    /// <summary>
    /// Set the icon of the tournament
    /// </summary>
    public bool SetIcon(string? icon)
    {
        return new StringValidator()
            .AllowNull()
            .Trim()
            .SetMinimumLength(16)
            .SetMaximumLength(16)
            .SetInvalidRegex(new(@"[^\w]"))
            .OnSuccess(icon => Icon = icon ?? DefaultIcon)
            .Test(icon);
    }

    /// <summary>
    /// Set the banner of the tournament
    /// </summary>
    public bool SetBanner(string? banner)
    {
        return new StringValidator()
            .AllowNull()
            .Trim()
            .SetMinimumLength(16)
            .SetMaximumLength(16)
            .SetInvalidRegex(new(@"[^\w]"))
            .OnSuccess(banner => Banner = banner ?? DefaultBanner)
            .Test(banner);
    }

    /// <summary>
    /// Add a social link to the tournament
    /// </summary>
    public bool AddSocial(Social social)
    {
        if (Socials.ContainsKey(social.Id))
            return false;

        Socials.Add(social.Id, social);
        return true;
    }

    /// <summary>
    /// Remove a social link from the tournament
    /// </summary>
    public bool RemoveSocial(string id)
    {
        if (!Socials.ContainsKey(id))
            return false;

        Socials.Remove(id);
        return true;
    }

    /// <summary>
    /// Mark the tournament as verified
    /// </summary>
    public void Verify()
    {
        Verified = true;
    }
}
