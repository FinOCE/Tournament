namespace Tournament.Models;

/// <summary>
/// A team that comprises of members
/// </summary>
public class Team
{
    public static readonly int NameMinLength = 1;
    public static readonly int NameMaxLength = 24;
    public static readonly Regex NameInvalidRegex = new(@"[^\w-. ]");

    public string Id { get; init; }
    public string Name { get; private set; }
    public TeamMember[] Members { get; private set; }

    public Team(string id, string name)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (!SetName(name))
            throw new ArgumentException($"Invalid {nameof(name)} provided");

        // Assign arguments to team
        Id = id;
        Name = name;
        Members = Array.Empty<TeamMember>();
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
    /// Add a member to the team
    /// </summary>
    public bool AddMember(TeamMember member)
    {
        if (Members.Where(m => m.User.Id == member.User.Id).ToArray().Length != 0)
            return false;

        Members = Members.Append(member).ToArray();
        return true;
    }

    /// <summary>
    /// Add a user to the team with a given role
    /// </summary>
    public bool AddMember(User user, int roles = (int)TeamRole.Player)
    {
        if (Members.Where(m => m.User.Id == user.Id).ToArray().Length != 0)
            return false;

        Members = Members.Append(new TeamMember(user, this, roles)).ToArray();
        return true;
    }

    /// <summary>
    /// Remove a member by their snowflake ID
    /// </summary>
    public bool RemoveMember(string id)
    {
        int memberCount = Members.Length;
        Members = Members.Where(m => m.User.Id != id).ToArray();

        return memberCount > Members.Length;
    }
}
