namespace API.Models.Brackets.Progressions;

public interface IProgression
{
    Dictionary<string, ITeam> Teams { get; }

    /// <summary>
    /// Add the team to the progression
    /// </summary>
    bool AddTeam(ITeam team);

    /// <summary>
    /// Remove the team from the progression
    /// </summary>
    bool RemoveTeam(string id);
}
