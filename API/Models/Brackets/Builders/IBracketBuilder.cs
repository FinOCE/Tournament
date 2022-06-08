namespace API.Models.Brackets.Builders;

public interface IBracketBuilder
{
    public string Id { get; }
    Dictionary<string, ITeam> Teams { get; }
    Dictionary<string, int> Seeds { get; }

    /// <summary>
    /// Add a team to the bracket
    /// </summary>
    bool AddTeam(ITeam team, int seed = 0);

    /// <summary>
    /// Remove a team from the bracket
    /// </summary>
    bool RemoveTeam(string id);

    /// <summary>
    /// Set the seed of a team
    /// </summary>
    bool SetSeed(string id, int seed = 0);

    /// <summary>
    /// Set the default maximum number of games per series
    /// </summary>
    bool SetBestOf(int bestOf);

    /// <summary>
    /// Generate the bracket
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    IStructure Generate();

    /// <summary>
    /// Sort the teams by their seeding and return as an array in order of best to worst
    /// </summary>
    ITeam[] GetOrderedTeams();
}
