namespace API.Models.Users;

/// <summary>
/// A user that is a member of a team
/// </summary>
public class TeamMember
{
    public User User { get; init; }
    public ITeam Team { get; init; }
    private int _Roles { get; set; }

    public TeamMember(User user, ITeam team, int roles = (int)TeamRole.Player)
    {
        User = user;
        Team = team;
        _Roles = roles;
    }

    /// <summary>
    /// Check if the member has a role
    /// </summary>
    public bool HasRole(TeamRole role)
    {
        return (int)role == (_Roles & (int)role);
    }

    /// <summary>
    /// Give a role to the member
    /// </summary>
    public void AddRole(TeamRole role)
    {
        _Roles |= (int)role;
    }

    /// <summary>
    /// Remove a role from the member
    /// </summary>
    public void RemoveRole(TeamRole role)
    {
        _Roles &= ~(int)role;
    }

    /// <summary>
    /// Determine if the member has the given permission
    /// </summary>
    public bool HasPermission(TeamRolePermission permission)
    {
        switch (permission)
        {
            case TeamRolePermission.Substitute:
                return HasRole(TeamRole.Player)
                    || HasRole(TeamRole.Substitute);

            case TeamRolePermission.ManageRegistration:
                return HasRole(TeamRole.Owner)
                    || HasRole(TeamRole.Manager)
                    || HasRole(TeamRole.Coach)
                    || HasRole(TeamRole.Captain);

            case TeamRolePermission.ManageTeam:
                return HasRole(TeamRole.Owner)
                    || HasRole(TeamRole.Manager);

            default:
                return false;
        }
    }
}

public enum TeamRole
{
    Owner = 1,
    Manager = 2,
    Coach = 4,
    Captain = 8,
    Player = 16,
    Substitute = 32
}

public enum TeamRolePermission
{
    Substitute,         // Allows member to remove themselves from competing in an event
    ManageRegistration, // Allows member to register for events and choose members to compete
    ManageTeam          // Allows member to modify team information
}