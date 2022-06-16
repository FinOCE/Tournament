namespace API.Models.Brackets.Builders;

public interface IBracketBuilder
{
    public string Id { get; }
    Dictionary<string, ITeam> Teams { get; }
    Dictionary<string, int> Seeds { get; }
    int BestOf { get; }
    IStructure? Bracket { get; }
    bool Private { get; }
    Dictionary<string, BracketInvite> Invites { get; }

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
    bool SetSeed(string id, int seed = BracketBuilder.DefaultSeed);

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
    /// Set the bracket to a root
    /// </summary>
    /// <param name="root">The root of the structure for the bracket</param>
    void SetBracket(IStructure? root = null);

    /// <summary>
    /// Sort the teams by their seeding and return as an array in order of best to worst
    /// </summary>
    ITeam[] GetOrderedTeams();

    /// <summary>
    /// Make a bracket invite-only
    /// </summary>
    void Privatize(bool priv = false);

    /// <summary>
    /// Add an invite to the bracket
    /// </summary>
    /// <param name="invite">The invite to be added</param>
    /// <returns>Whether or not the invite was successfully added</returns>
    bool AddInvite(BracketInvite invite);

    /// <summary>
    /// Remove an invite from the bracket
    /// </summary>
    /// <param name="id">The ID of the invite to be removed</param>
    /// <returns>Whether or not the invite was successfully removed</returns>
    bool RemoveInvite(string id);
}
