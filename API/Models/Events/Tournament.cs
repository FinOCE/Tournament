namespace API.Models.Events;

/// <summary>
/// A tournament that can include events
/// </summary>
public class Tournament
{
    public static readonly int NameMinLength = 3;
    public static readonly int NameMaxLength = 32;
    public static readonly Regex NameInvalidRegex = new(@"[^\w-. ]");
    public static readonly int DescriptionMaxLength = 2048;
    public static readonly Regex DescriptionInvalidRegex = new(@"[^\w-. ]");
    public static readonly int RulesMaxLength = 2048;
    public static readonly Regex RulesInvalidRegex = new(@"[^\w-. ]");
    public static readonly int IconNameLength = 16;
    public static readonly Regex IconNameInvalidRegex = new(@"[^\w]");
    public static readonly int BannerNameLength = 16;
    public static readonly Regex BannerNameInvalidRegex = new(@"[^\w]");
    public const string DefaultIcon = ""; // TODO: Create default icon path
    public const string DefaultBanner = ""; // TODO: Create default banner path

    public string Id { get; init; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Rules { get; private set; }
    public Dictionary<string, Coordinator> Coordinators { get; init; }
    public string Icon { get; private set; }
    public string Banner { get; private set; }
    // TODO: Include property for the game (new class)
    // TODO: Include a dictionary of events (new class)
    // TODO: Include links to socials (new class)

    /// <exception cref="ArgumentException"></exception>
    public Tournament(
        string id,
        string name,
        string? description = null,
        string? rules = null,
        Dictionary<string, Coordinator>? coordinators = null,
        string? icon = DefaultIcon,
        string? banner = DefaultBanner)
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
    }

    // TODO: Create validator builder to avoid so much duplicated code

    /// <summary>
    /// Set the name of the tournament
    /// </summary>
    public bool SetName(string name)
    {
        // Validate
        name = name.Trim();

        if (name.Length > NameMaxLength || name.Length < NameMinLength)
            return false;

        if (NameInvalidRegex.IsMatch(name))
            return false;

        // Update
        Name = name;
        return true;
    }

    /// <summary>
    /// Set the description of the tournament
    /// </summary>
    public bool SetDescription(string? description)
    {
        // Allow null
        if (description is null)
        {
            Description = null;
            return true;
        }

        // Validate
        description = description.Trim();

        if (description.Length > DescriptionMaxLength)
            return false;

        if (DescriptionInvalidRegex.IsMatch(description))
            return false;

        // Update
        Description = description;
        return true;
    }

    /// <summary>
    /// Set the rules of the tournament
    /// </summary>
    public bool SetRules(string? rules)
    {
        // Allow null
        if (rules is null)
        {
            Rules = null;
            return true;
        }

        // Validate
        rules = rules.Trim();

        if (rules.Length > RulesMaxLength)
            return false;

        if (RulesInvalidRegex.IsMatch(rules))
            return false;

        // Update
        Rules = rules;
        return true;
    }

    /// <summary>
    /// Set the icon of the tournament
    /// </summary>
    public bool SetIcon(string? icon)
    {
        // Allow null
        if (icon is null)
        {
            Icon = DefaultIcon;
            return true;
        }

        // Validate
        icon = icon.Trim();

        if (icon.Length > IconNameLength)
            return false;

        if (IconNameInvalidRegex.IsMatch(icon))
            return false;

        // Update
        Icon = icon;
        return true;
    }

    /// <summary>
    /// Set the banner of the tournament
    /// </summary>
    public bool SetBanner(string? banner)
    {
        // Allow null
        if (banner is null)
        {
            Icon = DefaultBanner;
            return true;
        }

        // Validate
        banner = banner.Trim();

        if (banner.Length > BannerNameLength)
            return false;

        if (BannerNameInvalidRegex.IsMatch(banner))
            return false;

        // Update
        Banner = banner;
        return true;
    }
}
