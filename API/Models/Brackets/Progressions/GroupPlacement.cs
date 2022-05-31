namespace API.Models.Brackets.Progressions;

/// <summary>
/// A placement where a team ends
/// </summary>
public class GroupPlacement : IPlacement
{
    public Dictionary<string, Team> Teams { get; init; } = new();
    public int Position { get; init; }

    public GroupPlacement(int position)
    {
        Position = position;
    }

    public bool AddTeam(Team team)
    {
        if (Teams.ContainsKey(team.Id))
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
