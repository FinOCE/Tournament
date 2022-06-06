namespace API.Models.Teams;

/// <summary>
/// A team that comprises of members
/// </summary>
public class Team : ITeam
{
    public const string DefaultIcon = ""; // TODO: Create default icon path

    public string Id { get; init; }
    public string Name { get; private set; }
    public string Icon { get; private set; }
    public bool Verified { get; private set; }
    public Dictionary<string, TeamMember> Members { get; private set; }

    /// <exception cref="ArgumentException"></exception>
    public Team(string id, string name, string? icon, bool verified)
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
        Verified = verified;
        Members = new();
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
    /// Mark the team as verified
    /// </summary>
    public void Verify()
    {
        Verified = true;
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
}
