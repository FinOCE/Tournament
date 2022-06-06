namespace API.Models.Users;

/// <summary>
/// A user of the service
/// </summary>
public class User
{
    public static readonly int UsernameMinLength = 3;
    public static readonly int UsernameMaxLength = 16;
    public static readonly Regex UsernameInvalidRegex = new(@"[^\w-. ]");
    public static readonly int IconNameLength = 16;
    public static readonly Regex IconNameInvalidRegex = new(@"[^\w]");
    public static readonly string DefaultIcon = string.Empty; // TODO: Create default icon path

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
