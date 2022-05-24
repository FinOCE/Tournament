namespace Tournament.Models;

/// <summary>
/// A user of the service
/// </summary>
public class User
{
    public string Id { get; init; }
    public string Username { get; private set; }
    public DateTime Timestamp { get; init; }
    private int _Permissions { get; set; }

    public User(string id, string username, int permissions = 0)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (!SetUsername(username))
            throw new ArgumentException($"Invalid {nameof(username)} provided");

        // Assign arguments to user
        Id = id;
        Username = username;
        Timestamp = Snowflake.GetTimestamp(id);
        _Permissions = permissions;
    }

    /// <summary>
    /// Update the user's username
    /// </summary>
    public bool SetUsername(string username)
    {
        // Validate username
        username = username.Trim();

        if (username.Length > 16 || username.Length < 3)
            return false;

        if (new Regex(@"[^\w-. ]").IsMatch(username))
            return false;

        // Update username
        Username = username;
        return true;
    }

    /// <summary>
    /// Check if the user has a permission
    /// </summary>
    public bool HasPermission(UserPermission permission)
    {
        return (int)permission == (_Permissions & (int)permission);
    }

    /// <summary>
    /// Give a permission to the user
    /// </summary>
    public void AddPermission(UserPermission permission)
    {
        _Permissions |= (int)permission;
    }

    /// <summary>
    /// Remove a permission from the user
    /// </summary>
    public void RemovePermission(UserPermission permission)
    {
        _Permissions &= ~(int)permission;
    }
}

public enum UserPermission
{
    Administrator = 1
    // Additional permissions can be added as as 2^x
}
