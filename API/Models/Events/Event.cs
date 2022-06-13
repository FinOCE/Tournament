namespace API.Models.Events;

/// <summary>
/// An event that is run in a tournament
/// </summary>
public class Event : IRewardable
{
    public const string DefaultIcon = "1234567890123456"; // TODO: Create default icon path
    public const string DefaultBanner = "1234567890123456"; // TODO: Create default banner path

    private IRewardable _Rewardable = new Rewardable();

    public string Id { get; init; }
    public string Name { get; private set; }
    public Tournament Tournament { get; init; }
    public Dictionary<string, IBracketBuilder> Brackets { get; init; }
    public bool BracketsVisible { get; private set; }
    public DateTime StartTimestamp { get; private set; }
    public DateTime RegistrationTimestamp { get; private set; }
    public DateTime? FinishedTimestamp { get; private set; }
    public bool Finished { get { return FinishedTimestamp is not null; } }
    public string Icon { get; private set; }
    public string Banner { get; private set; }
    public Dictionary<string, Prize> Prizes { get { return _Rewardable.Prizes; } }

    /// <exception cref="ArgumentException"></exception>
    public Event(
        string id,
        string name,
        Tournament tournament,
        DateTime startTimestamp,
        DateTime? registrationTimestamp = null,
        DateTime? finishedTimestamp = null,
        Dictionary<string, IBracketBuilder>? brackets = null,
        bool bracketsVisible = true,
        string? icon = DefaultIcon,
        string? banner = DefaultBanner,
        Dictionary<string, Prize>? prizes = null)
    {
        // Validate
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (!SetName(name))
            throw new ArgumentException($"Invalid {nameof(name)} provided");

        if (registrationTimestamp is not null && registrationTimestamp > startTimestamp)
            throw new ArgumentException($"Registration cannot close after the start time");

        if (finishedTimestamp is not null && finishedTimestamp < startTimestamp)
            throw new ArgumentException($"The event cannot finish before the start time");

        // TODO: Validate that all brackets are finished if the finishedTimestamp exists

        // Instantiate
        Id = id;
        Name = name;
        Tournament = tournament;
        Brackets = brackets ?? new();
        StartTimestamp = startTimestamp;
        RegistrationTimestamp = registrationTimestamp ?? startTimestamp;
        FinishedTimestamp = finishedTimestamp;
        BracketsVisible = bracketsVisible;
        Icon = icon ?? DefaultIcon;
        Banner = banner ?? DefaultBanner;

        if (prizes is not null)
            SetPrizes(prizes);
    }

    /// <summary>
    /// Set the name of the event
    /// </summary>
    /// <param name="name">What the event should be called</param>
    /// <returns>Whether or not the name was successfully changed</returns>
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
    /// Set the timestamp to start the event
    /// </summary>
    /// <param name="timestamp">The new start time</param>
    /// <returns>Whether or not the timestamp was successfully updated</returns>
    public bool SetStartTimestamp(DateTime timestamp)
    {
        if (timestamp < DateTime.UtcNow)
            return false;

        if (StartTimestamp < DateTime.UtcNow)
            return false;

        if (RegistrationTimestamp > timestamp)
            RegistrationTimestamp = timestamp;

        StartTimestamp = timestamp;
        return true;
    }

    /// <summary>
    /// Set the timestamp for registration to close
    /// </summary>
    /// <param name="timestamp">The new registration close time</param>
    /// <returns>Whether or not the timestamp was successfully updated</returns>
    public bool SetRegistrationTimestamp(DateTime timestamp)
    {
        if (timestamp < DateTime.UtcNow)
            return false;

        if (StartTimestamp < DateTime.UtcNow)
            return false;

        if (StartTimestamp < timestamp)
            StartTimestamp = timestamp;

        RegistrationTimestamp = timestamp;
        return true;
    }

    /// <summary>
    /// Add a bracket to the event
    /// </summary>
    /// <param name="bracket">The bracket to be added</param>
    /// <returns>Whether or not the bracket was successfully added</returns>
    public bool AddBracket(IBracketBuilder bracket)
    {
        if (Brackets.ContainsKey(bracket.Id))
            return false;

        Brackets.Add(bracket.Id, bracket);
        return true;
    }

    /// <summary>
    /// Remove a bracket from the event
    /// </summary>
    /// <param name="id">The ID of the bracket to be removed</param>
    /// <returns>Whether or not the bracket was successfully removed</returns>
    public bool RemoveBracket(string id)
    {
        if (!Brackets.ContainsKey(id))
            return false;

        Brackets.Remove(id);
        return true;
    }

    /// <summary>
    /// Set the icon of the event
    /// </summary>
    /// <param name="icon">The icon to be used</param>
    /// <returns>Whether or not the icon was successfully set</returns>
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
    /// Set the banner of the event
    /// </summary>
    /// <param name="banner">The banner to be used</param>
    /// <returns>Whether or not the banner was successfully set</returns>
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
    /// Finish the event
    /// </summary>
    /// <returns>Whether or not the event was successfully finished</returns>
    public bool Finish()
    {
        if (Finished)
            return false;

        // TODO: Validate the event finishing based on the brackets

        FinishedTimestamp = DateTime.UtcNow;
        return true;
    }

    public void SetPrizes(Dictionary<string, Prize> prizes)
    {
        _Rewardable.SetPrizes(prizes);
    }

    public bool AddPrize(Prize prize)
    {
        return _Rewardable.AddPrize(prize);
    }

    public bool RemovePrize(string id)
    {
        return _Rewardable.RemovePrize(id);
    }
}
