namespace API.Models.Brackets.Progressions;

/// <summary>
/// A placement where a team ends without a specific position
/// </summary>
public class GroupPlacement : IPlacement
{
    public Dictionary<string, Team> Teams { get; init; } = new();
    public int Position { get; init; }

    /// <exception cref="ArgumentException"></exception>
    public GroupPlacement(int position)
    {
        if (position < 1)
            throw new ArgumentException($"Invalid {nameof(position)} provided");

        Position = position;
    }

    public bool AddTeam(Team team)
    {
        if (Teams.ContainsKey(team.Id))
            return false;

        if (Teams.Count + 1 > Position)
            return false;

        Teams.Add(team.Id, team);
        return true;
    }

    public bool RemoveTeam(string id)
    {
        if (!Teams.ContainsKey(id))
            return false;

        Teams.Remove(id);
        return true;
    }

    public override string ToString()
    {
        return "Top " + Position;
    }
}
