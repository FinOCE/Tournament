namespace API.Models.Teams;

/// <summary>
/// A team that comprises of members
/// </summary>
public class Team : ITeam, ISociable, IVerifiable
{
    public const string DefaultIcon = ""; // TODO: Create default icon path

    private ISociable _Sociable = new Sociable();
    private IVerifiable _Verifiable = new Verifiable();

    public string Id { get; init; }
    public string Name { get; private set; }
    public string Icon { get; private set; }
    public Dictionary<string, TeamMember> Members { get; private set; }
    public Dictionary<string, Social> Socials { get { return _Sociable.Socials; } }
    public bool Verified { get { return _Verifiable.Verified; } }


    /// <exception cref="ArgumentException"></exception>
    public Team(
        string id,
        string name,
        string? icon = null,
        bool verified = false,
        Dictionary<string,Social>? socials = null)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (!SetName(name))
            throw new ArgumentException($"Invalid {nameof(name)} provided");

        // Assign arguments to team
        Id = id;
        Name = name;
        Icon = icon ?? DefaultIcon;
        Members = new();

        if (socials is not null)
            SetSocials(socials);

        if (verified)
            Verify();
    }

    /// <summary>
    /// Update a team's name
    /// </summary>
    public bool SetName(string name)
    {
        return new StringValidator()
            .Trim()
            .SetMinimumLength(1)
            .SetMaximumLength(24)
            .SetInvalidRegex(new(@"[^\w-. ]"))
            .OnSuccess(name => Name = name!)
            .Test(name);
    }

    /// <summary>
    /// Set whether the default icon or a brand icon is used
    /// </summary>
    public bool SetIcon(string? icon)
    {
        return new StringValidator()
            .AllowNull()
            .SetMinimumLength(16)
            .SetMaximumLength(16)
            .SetInvalidRegex(new(@"[^\w]"))
            .OnSuccess(icon => Icon = icon ?? DefaultIcon)
            .Test(icon);
    }

    /// <summary>
    /// Add a member to the team
    /// </summary>
    public bool AddMember(TeamMember member)
    {
        if (Members.ContainsKey(member.User.Id))
            return false;

        Members.Add(member.User.Id, member);
        return true;
    }

    /// <summary>
    /// Add a user to the team with a given role
    /// </summary>
    public bool AddMember(User user, int roles = (int)TeamRole.Player)
    {
        if (Members.ContainsKey(user.Id))
            return false;

        Members.Add(user.Id, new TeamMember(user, this, roles));
        return true;
    }

    /// <summary>
    /// Remove a member by their snowflake ID
    /// </summary>
    public bool RemoveMember(string id)
    {
        if (!Members.ContainsKey(id))
            return false;

        Members.Remove(id);
        return true;
    }

    public void SetSocials(Dictionary<string, Social> socials)
    {
        _Sociable.SetSocials(socials);
    }

    public bool AddSocial(Social social)
    {
        return _Sociable.AddSocial(social);
    }

    public bool RemoveSocial(string id)
    {
        return _Sociable.RemoveSocial(id);
    }

    public void Verify()
    {
        _Verifiable.Verify();
    }
}
