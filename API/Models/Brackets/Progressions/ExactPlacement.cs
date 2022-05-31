namespace API.Models.Brackets.Progressions;

/// <summary>
/// A placement where the exact overall position is known
/// </summary>
public class ExactPlacement : IPlacement
{
    public Dictionary<string, Team> Teams { get; init; } = new();
    public int Position { get; init; }

    public ExactPlacement(int position)
    {
        Position = position;
    }

    public bool AddTeam(Team team)
    {
        if (Teams.Keys.Count == 1)
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
        // Determine suffix
        int offset = Math.Max(0, Position.ToString().Length - 2);
        string suffix = Position.ToString()[offset..] switch
        {
            "11" => "th",
            "12" => "th",
            _ => Position.ToString().Last() switch
            {
                '1' => "st",
                '2' => "nd",
                '3' => "rd",
                _ => "th"
            }
        };

        // Return label for position
        return Position + suffix;
    }
}
