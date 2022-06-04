namespace API.Models.Brackets.Builders;

/// <summary>
/// A builder to generate a bracket
/// </summary>
public abstract class BracketBuilder : IBracketBuilder
{
    protected SnowflakeService SnowflakeService { get; init; }
    public Dictionary<string, Team> Teams { get; init; } = new();
    public Dictionary<string, int> Seeds { get; init; } = new();
    public int BestOf { get; private set; } = 1;

    public BracketBuilder(SnowflakeService snowflakeService)
    {
        SnowflakeService = snowflakeService;
    }

    public virtual bool AddTeam(Team team, int seed = 0)
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

    /// <summary>
    /// Sort the teams by their seeding and return as an array in order of best to worst
    /// </summary>
    public virtual Team[] GetOrderedTeams()
    {
        return Teams
            .OrderBy(team => Seeds[team.Key])
            .Reverse()
            .Select(kvp => kvp.Value)
            .ToArray();
    }
}
