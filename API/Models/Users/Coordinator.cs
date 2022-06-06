namespace API.Models.Users;

/// <summary>
/// A coordinator of a tournament
/// </summary>
public class Coordinator
{
    public User User { get; init; }
    public Tournament Tournament { get; init; }
    private int _Roles { get; set; }

    public Coordinator(User user, Tournament tournament, int roles = (int)HostRole.Moderator)
    {
        User = user;
        Tournament = tournament;
        _Roles = roles;
    }

    /// <summary>
    /// Check if the host has a role
    /// </summary>
    public bool HasRole(HostRole role)
    {
        return (int)role == (_Roles & (int)role);
    }

    /// <summary>
    /// Give a role to the host
    /// </summary>
    public void AddRole(HostRole role)
    {
        _Roles |= (int)role;
    }

    /// <summary>
    /// Remove a role from the host
    /// </summary>
    public void RemoveRole(HostRole role)
    {
        _Roles &= ~(int)role;
    }

    /// <summary>
    /// Determine if the host has the given permission
    /// </summary>
    public bool HasPermission(HostRolePermission permission)
    {
        switch (permission)
        {
            case HostRolePermission.Spectate:
                return HasRole(HostRole.Owner)
                    || HasRole(HostRole.Administrator)
                    || HasRole(HostRole.Moderator)
                    || HasRole(HostRole.Caster);

            case HostRolePermission.ManageSeries:
                return HasRole(HostRole.Owner)
                    || HasRole(HostRole.Administrator)
                    || HasRole(HostRole.Moderator);

            case HostRolePermission.ManageRegistration:
                return HasRole(HostRole.Owner)
                    || HasRole(HostRole.Administrator);

            case HostRolePermission.ManageSettings:
                return HasRole(HostRole.Owner);

            default:
                return false;
        }
    }
}

public enum HostRole
{
    Owner = 1,
    Administrator = 2,
    Moderator = 4,
    Caster = 8
}

public enum HostRolePermission
{
    Spectate,           // Allows host to view chat and lobby details
    ManageSeries,       // Allows host to manage aspects of a series
    ManageRegistration, // Allows host to CRUD team registrations
    ManageSettings      // Allows host to change all aspects of the event
}
