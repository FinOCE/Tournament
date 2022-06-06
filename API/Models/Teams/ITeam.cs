namespace API.Models.Teams;

public interface ITeam
{
    string Id { get; }
    string Name { get; }
    string Icon { get; }
    bool Verified { get; }
    Dictionary<string, TeamMember> Members { get; }
}