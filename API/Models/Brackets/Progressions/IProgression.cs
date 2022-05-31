namespace API.Models.Brackets.Progressions;

public interface IProgression
{
    Dictionary<string, Team> Teams { get; }

    /// <summary>
    /// Add the team to the progression
    /// </summary>
    bool AddTeam(Team team);

    /// <summary>
    /// Remove the team from the progression
    /// </summary>
    bool RemoveTeam(string id);
}
