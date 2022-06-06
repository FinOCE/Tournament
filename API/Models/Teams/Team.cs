namespace API.Models.Teams;

/// <summary>
/// A team that comprises of members
/// </summary>
public class Team : ITeam
{
    public static readonly int NameMinLength = 1;
    public static readonly int NameMaxLength = 24;
    public static readonly Regex NameInvalidRegex = new(@"[^\w-. ]");
    public static readonly int IconNameLength = 16;
    public static readonly Regex IconNameInvalidRegex = new(@"[^\w]");
    public static readonly string DefaultIcon = string.Empty; // TODO: Create default icon path

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
        // Validate team name
        name = name.Trim();

        if (name.Length > NameMaxLength || name.Length < NameMinLength)
            return false;

        if (NameInvalidRegex.IsMatch(name))
            return false;

        // Update name
        Name = name;
        return true;
    }

    /// <summary>
    /// Set whether the default icon or a brand icon is used
    /// </summary>
    public bool SetIcon(string? icon)
    {
        // Validate icon
        if (icon is null)
        {
            Icon = DefaultIcon;
            return true;
        }

        if (icon.Length != IconNameLength)
            return false;

        if (IconNameInvalidRegex.IsMatch(icon))
            return false;

        // Update icon
        Icon = icon ?? DefaultIcon;
        return true;
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
