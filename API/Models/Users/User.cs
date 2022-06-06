namespace API.Models.Users;

/// <summary>
/// A user of the service
/// </summary>
public class User
{
    public const string DefaultIcon = "1234567890123456"; // TODO: Create default icon path

    public string Id { get; init; }
    public string Username { get; private set; }
    public int Discriminator { get; private set; }
    public string Icon { get; private set; }
    public bool Verified { get; private set; }
    public DateTime Timestamp { get; init; }
    private int _Permissions { get; set; }

    public string Tag
    {
        get { return $"{Username}#{Discriminator:0000}"; }
    }

    /// <exception cref="ArgumentException"></exception>
    public User(string id, string username, int discriminator, string? icon = null, bool verified = false, int permissions = 0)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (!SetUsername(username))
            throw new ArgumentException($"Invalid {nameof(username)} provided");

        if (!SetDiscriminator(discriminator))
            throw new ArgumentException($"Invalid {nameof(discriminator)} provided");

        if (!SetIcon(icon))
            throw new ArgumentException($"Invalid {nameof(icon)} provided");

        // Assign arguments to user
        Id = id;
        Username = username;
        Icon = icon ?? DefaultIcon;
        Verified = verified;
        Discriminator = discriminator;
        Timestamp = Snowflake.GetTimestamp(id);
        _Permissions = permissions;
    }

    /// <summary>
    /// Update the user's username
    /// </summary>
    public bool SetUsername(string username)
    {
        return new StringValidator()
            .Trim()
            .SetMinimumLength(3)
            .SetMaximumLength(16)
            .SetInvalidRegex(new(@"[^\w-. ]"))
            .OnSuccess((string? username) => Username = username!)
            .Test(username);
    }

    /// <summary>
    /// Update the user's discriminator
    /// </summary>
    public bool SetDiscriminator(int discriminator)
    {
        // Validate discriminator
        if (discriminator < 1 || discriminator > 9999)
            return false;

        // Update discriminator
        Discriminator = discriminator;
        return true;
    }

    /// <summary>
    /// Set whether the default icon or a user icon is used
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
    /// Mark the user as verified
    /// </summary>
    public void Verify()
    {
        Verified = true;
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
