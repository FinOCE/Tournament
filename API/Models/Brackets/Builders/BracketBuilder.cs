namespace API.Models.Brackets.Builders;

/// <summary>
/// A builder to generate a bracket
/// </summary>
public abstract class BracketBuilder : IBracketBuilder, IProgression
{
    public const int DefaultSeed = 0;

    public string Id { get; init; }
    protected SnowflakeService SnowflakeService { get; init; }
    public Dictionary<string, ITeam> Teams { get; init; }
    public Dictionary<string, int> Seeds { get; init; }
    public int BestOf { get; protected set; }
    public IStructure? Bracket { get; protected set; }
    public bool Private { get; private set; }
    public Dictionary<string, BracketInvite> Invites { get; init; }

    /// <exception cref="ArgumentException"></exception>
    public BracketBuilder(
        string id,
        SnowflakeService snowflakeService,
        Dictionary<string, ITeam>? teams = null,
        Dictionary<string, int>? seeds = null,
        int bestOf = 1,
        bool priv = false,
        Dictionary<string, BracketInvite>? invites = null)
    {
        // Validate
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        // Instantiate
        Id = id;
        SnowflakeService = snowflakeService;
        Teams = teams ?? new();
        Seeds = seeds ?? new();
        BestOf = bestOf;
        Private = priv;
        Invites = invites ?? new();
    }

    public virtual bool AddTeam(ITeam team)
    {
        return AddTeam(team, DefaultSeed);
    }

    public virtual bool AddTeam(ITeam team, int seed = DefaultSeed)
    {
        if (Private && !Invites.Values.Any(i => i.Team.Id == team.Id))
            return false;

        if (Teams.ContainsKey(team.Id))
            return false;

        Teams.Add(team.Id, team);
        Seeds.Add(team.Id, seed);
        return true;
    }

    public virtual bool RemoveTeam(string id)
    {
        if (!Teams.ContainsKey(id))
            return false;

        Teams.Remove(id);
        Seeds.Remove(id);
        return true;
    }
    
    public virtual bool SetSeed(string id, int seed = DefaultSeed)
    {
        if (!Teams.ContainsKey(id))
            return false;

        Seeds[id] = seed;
        return true;
    }

    public virtual bool SetBestOf(int bestOf)
    {
        if (bestOf < 1)
            return false;

        BestOf = bestOf;
        return true;
    }

    public abstract IStructure Generate();

    public virtual ITeam[] GetOrderedTeams()
    {
        return Teams
            .OrderBy(team => Seeds[team.Key])
            .Reverse()
            .Select(kvp => kvp.Value)
            .ToArray();
    }

    public virtual void Privatize(bool priv = true)
    {
        Private = priv;
    }

    public virtual bool AddInvite(BracketInvite invite)
    {
        if (Invites.ContainsKey(invite.Id))
            return false;

        Invites.Add(invite.Id, invite);
        return true;
    }

    public virtual bool RemoveInvite(string id)
    {
        if (!Invites.ContainsKey(id))
            return false;

        Invites.Remove(id);
        return true;
    }
}
