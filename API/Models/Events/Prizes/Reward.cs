namespace API.Models.Events.Prizes;

public class Reward
{
    public string Id { get; init; }
    public int Amount { get; private set; }
    public string? Name { get; private set; }

    public Reward(string id, int amount = 1, string? name = null)
    {
        // Validate
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (!SetAmount(amount))
            throw new ArgumentException($"Invalid {nameof(amount)} provided");

        if (!SetName(name))
            throw new ArgumentException($"Invalid {nameof(name)} provided");

        // Instantiate
        Id = id;
        Amount = amount;
        Name = name;
    }

    /// <summary>
    /// Set the amount of the reward to give
    /// </summary>
    /// <param name="amount">The amount to give that is greater than 0</param>
    /// <returns>Whether or not the amount was successfully updated</returns>
    public bool SetAmount(int amount)
    {
        if (amount <= 0)
            return false;

        Amount = amount;
        return true;
    }

    /// <summary>
    /// Set the name of the reward to give
    /// </summary>
    /// <param name="name">The name of the reward</param>
    /// <returns>Whether or not the name was successfully updated</returns>
    public bool SetName(string? name)
    {
        return new StringValidator()
            .Trim()
            .AllowNull()
            .SetMinimumLength(1)
            .SetMaximumLength(64)
            .OnSuccess(name => Name = name!)
            .Test(name);
    }
}
