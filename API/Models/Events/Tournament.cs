namespace API.Models.Events;

/// <summary>
/// A tournament that can include events
/// </summary>
public class Tournament : ISociable, IVerifiable
{
    public const string DefaultIcon = "1234567890123456"; // TODO: Create default icon path
    public const string DefaultBanner = "1234567890123456"; // TODO: Create default banner path

    private ISociable _Sociable = new Sociable();
    private IVerifiable _Verifiable = new Verifiable();

    public string Id { get; init; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Rules { get; private set; }
    public Dictionary<string, Coordinator> Coordinators { get; init; }
    public string Icon { get; private set; }
    public string Banner { get; private set; }
    public Game? Game { get; private set; }
    public Dictionary<string, Prize> Prizes { get; init; }
    // TODO: Include a dictionary of events (new class)
    public Dictionary<string, Social> Socials { get { return _Sociable.Socials; } }
    public bool Verified { get { return _Verifiable.Verified; } }

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
        Dictionary<string, Prize>? prizes = null,
        Dictionary<string, Social>? socials = null,
        bool verified = false)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

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
        Prizes = prizes ?? new();

        if (socials is not null)
            SetSocials(socials);

        if (verified)
            Verify();
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
    /// Add a coordinator to the tournament
    /// </summary>
    /// <param name="coordinator">The coordinator to be added</param>
    /// <returns>Whether or not the coordinator was successfully added</returns>
    public bool AddCoordinator(Coordinator coordinator)
    {
        if (Coordinators.ContainsKey(coordinator.User.Id))
            return false;

        Coordinators.Add(coordinator.User.Id, coordinator);
        return true;
    }

    /// <summary>
    /// Removes a coordinator from the tournament
    /// </summary>
    /// <param name="id">The ID of the coordinator to be removed</param>
    /// <returns>Whether or not the coordinator was successfully removed</returns>
    public bool RemoveCoordinator(string id)
    {
        if (!Coordinators.ContainsKey(id))
            return false;

        Coordinators.Remove(id);
        return true;
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
    /// Set the game of the tournament
    /// </summary>
    /// <param name="game">The game the tournament is intended for</param>
    public void SetGame(Game game)
    {
        Game = game;
    }

    /// <summary>
    /// Add a prize to the tournament
    /// </summary>
    /// <param name="prize">The prize to be added</param>
    /// <returns>Whether or not the prize was successfully added</returns>
    public bool AddPrize(Prize prize)
    {
        if (Prizes.ContainsKey(prize.Id))
            return false;

        Prizes.Add(prize.Id, prize);
        return true;
    }

    /// <summary>
    /// Removes a prize from the tournament
    /// </summary>
    /// <param name="id">The ID of the prize to be removed</param>
    /// <returns>Whether or not the prize was successfully removed</returns>
    public bool RemovePrize(string id)
    {
        if (!Prizes.ContainsKey(id))
            return false;

        Prizes.Remove(id);
        return true;
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

    public void Verify()
    {
        _Verifiable.Verify();
    }
}
