namespace API.Models;

/// <summary>
/// A user of the service
/// </summary>
public class User
{
    public static readonly int UsernameMinLength = 3;
    public static readonly int UsernameMaxLength = 16;
    public static readonly Regex UsernameInvalidRegex = new(@"[^\w-. ]");

    public string Id { get; init; }
    public string Username { get; private set; }
    public DateTime Timestamp { get; init; }
    private int _Permissions { get; set; }

    /// <exception cref="ArgumentException"></exception>
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

        if (username.Length > UsernameMaxLength || username.Length < UsernameMinLength)
            return false;

        if (UsernameInvalidRegex.IsMatch(username))
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
}
