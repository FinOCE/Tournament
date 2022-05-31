namespace API.Models.Brackets.Builders;

/// <summary>
/// A builder to generate a bracket
/// </summary>
public class BracketBuilder : IBracketBuilder
{
    public Dictionary<string, Team> Teams { get; init; } = new();
    public Dictionary<string, int> Seeds { get; init; } = new();

    public BracketBuilder() { }

    public bool AddTeam(Team team, int seed = 0)
    {
        if (Teams.ContainsKey(team.Id))
            return false;

        Teams.Add(team.Id, team);
        Seeds.Add(team.Id, seed);
        return true;
    }

    public bool RemoveTeam(string id)
    {
        if (!Teams.ContainsKey(id))
            return false;

        Teams.Remove(id);
        Seeds.Remove(id);
        return true;
    }
    
    public bool SetSeed(string id, int seed = 0)
    {
        if (!Teams.ContainsKey(id))
            return false;

        Seeds[id] = seed;
        return true;
    }

    //public Series Generate()
    //{

    //}
}
