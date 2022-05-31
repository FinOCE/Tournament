namespace API.Models.Brackets.Builders;

public interface IBracketBuilder
{
    Dictionary<string, Team> Teams { get; }
    Dictionary<string, int> Seeds { get; }

    /// <summary>
    /// Add a team to the bracket
    /// </summary>
    bool AddTeam(Team team, int seed);

    /// <summary>
    /// Remove a team from the bracket
    /// </summary>
    bool RemoveTeam(string id);

    /// <summary>
    /// Set the seed of a team
    /// </summary>
    bool SetSeed(string id, int seed);

    ///// <summary>
    ///// Generate the bracket with the given options
    ///// </summary>
    //Series Generate();
}
