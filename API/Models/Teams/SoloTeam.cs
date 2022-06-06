namespace API.Models.Teams;

/// <summary>
/// A team that only contains one member
/// </summary>
public class SoloTeam : ITeam
{
    public string Id { get { return Members.Values.First().User.Id; } }
    public string Name { get { return Members.Values.First().User.Username; } }
    public string Icon { get { return Members.Values.First().User.Icon; } }
    public bool Verified { get { return Members.Values.First().User.Verified; } }
    public Dictionary<string, TeamMember> Members { get; init; } = new();

    public SoloTeam(User user)
    {
        TeamMember member = new(user, this, (int)TeamRole.Owner + (int)TeamRole.Player);
        Members.Add(user.Id, member);
    }
}
