namespace API.Models.Teams;

/// <summary>
/// A team that only contains one member
/// </summary>
public class SoloTeam : ITeam, ISociable, IVerifiable
{
    public string Id { get { return Members.Values.First().User.Id; } }
    public string Name { get { return Members.Values.First().User.Username; } }
    public string Icon { get { return Members.Values.First().User.Icon; } }
    public Dictionary<string, TeamMember> Members { get; init; } = new();
    public Dictionary<string, Social> Socials { get { return Members.Values.First().User.Socials; } }
    public bool Verified { get { return Members.Values.First().User.Verified; } }

    public SoloTeam(User user)
    {
        TeamMember member = new(user, this, (int)TeamRole.Owner + (int)TeamRole.Player);
        Members.Add(user.Id, member);
    }

    public void SetSocials(Dictionary<string, Social> socials)
    {
        Members.Values.First().User.SetSocials(socials);
    }

    public bool AddSocial(Social social)
    {
        return Members.Values.First().User.AddSocial(social);
    }

    public bool RemoveSocial(string id)
    {
        return Members.Values.First().User.RemoveSocial(id);
    }

    public void Verify()
    {
        Members.Values.First().User.Verify();
    }
}
