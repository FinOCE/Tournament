namespace API.Models.Socials;

/// <summary>
/// A video game
/// </summary>
public class Game : ISociable
{
    public const string DefaultIcon = "1234567890123456"; // TODO: Create default icon path
    public const string DefaultBanner = "1234567890123456"; // TODO: Create default icon banner

    private ISociable _Sociable = new Sociable();

    public string Id { get; init; }
    public string Name { get; private set; }
    public string[] Categories { get; private set; }
    public string? Description { get; private set; }
    public string Icon { get; private set; }
    public string Banner { get; private set; }
    public Dictionary<string, Social> Socials { get { return _Sociable.Socials; } }

    public Game(
        string id,
        string name,
        string[]? categories = null,
        string? description = null,
        string? icon = null,
        string? banner = null,
        Dictionary<string, Social>? socials = null)
    {
        // Validate
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (!SetName(name))
            throw new ArgumentException($"Invalid {nameof(name)} provided");

        if (categories is not null)
            foreach (string category in categories)
                if (!AddCategory(category))
                    throw new ArgumentException($"Invalid {nameof(categories)} provided");

        if (!SetDescription(description))
            throw new ArgumentException($"Invalid {nameof(description)} provided");

        if (!SetIcon(icon))
            throw new ArgumentException($"Invalid {nameof(icon)} provided");

        if (!SetBanner(banner))
            throw new ArgumentException($"Invalid {nameof(banner)} provided");

        // Instantiate
        Id = id;
        Name = name;
        Categories = categories ?? Array.Empty<string>();
        Description = description;
        Icon = icon ?? DefaultIcon;
        Banner = banner ?? DefaultBanner;

        if (socials is not null)
            SetSocials(socials);
    }

    /// <summary>
    /// Set the name of the game
    /// </summary>
    public bool SetName(string name)
    {
        return new StringValidator()
            .Trim()
            .SetMinimumLength(1)
            .SetMaximumLength(64)
            .OnSuccess(name => Name = name!)
            .Test(name);
    }

    /// <summary>
    /// Give the game a category
    /// </summary>
    public bool AddCategory(string category)
    {
        return new StringValidator()
            .Trim()
            .SetMinimumLength(1)
            .SetMaximumLength(16)
            .SetInvalidRegex(new(@"[^\w-. ]"))
            .OnSuccess(c => Categories = Categories.Append(c!).ToArray())
            .Test(category);
    }

    /// <summary>
    /// Remove a category from the game
    /// </summary>
    public bool RemoveCategory(string category)
    {
        if (!Categories.Contains(category))
            return false;

        Categories = Categories.Where(c => c != category).ToArray();
        return true;
    }

    /// <summary>
    /// Set the game's description
    /// </summary>
    public bool SetDescription(string? description = null)
    {
        return new StringValidator()
            .AllowNull()
            .Trim()
            .SetMinimumLength(1)
            .SetMaximumLength(2048)
            .OnSuccess(description => Description = description!)
            .Test(description);
    }

    /// <summary>
    /// Set the game icon
    /// </summary>
    public bool SetIcon(string? icon = null)
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
    /// Set the game banner
    /// </summary>
    public bool SetBanner(string? banner = null)
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

    public void SetSocials(Dictionary<string, Social> socials)
    {
        _Sociable.SetSocials(socials);
    }

    public bool AddSocial(Social social)
    {
        return _Sociable.AddSocial(social);
    }

    public bool RemoveSocial(string id)
    {
        return _Sociable.RemoveSocial(id);
    }
}
