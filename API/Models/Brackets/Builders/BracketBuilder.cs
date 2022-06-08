namespace API.Models.Brackets.Builders;

/// <summary>
/// A builder to generate a bracket
/// </summary>
public abstract class BracketBuilder : IBracketBuilder
{
    public string Id { get; init; }
    protected SnowflakeService SnowflakeService { get; init; }
    public Dictionary<string, ITeam> Teams { get; init; }
    public Dictionary<string, int> Seeds { get; init; }
    public int BestOf { get; private set; }
    // TODO: Handle progression between brackets

    /// <exception cref="ArgumentException"></exception>
    public BracketBuilder(
        string id,
        SnowflakeService snowflakeService,
        Dictionary<string, ITeam>? teams = null,
        Dictionary<string, int>? seeds = null,
        int bestOf = 1)
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

    }

    public virtual bool AddTeam(ITeam team, int seed = 0)
    {
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
    
    public virtual bool SetSeed(string id, int seed = 0)
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
}
