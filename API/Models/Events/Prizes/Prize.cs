namespace API.Models.Events.Prizes;

/// <summary>
/// A prize for winning an event or tournament
/// </summary>
public class Prize : IComparable<Prize>
{
    public string Id { get; init; }
    public IPlacement Placement { get; private set; }
    public Reward Reward;

    /// <exception cref="ArgumentException"></exception>
    public Prize(string id, IPlacement placement, Reward reward)
    {
        // Validate
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        // Instantiate
        Id = id;
        Placement = placement;
        Reward = reward;
    }

    /// <summary>
    /// Set the reward for the prize
    /// </summary>
    /// <param name="reward">The reward to set for this prize</param>
    public void SetReward(Reward reward)
    {
        Reward = reward;
    }

    public int CompareTo(Prize? prize)
    {
        if (prize is null)
            return -1;
        else if (Placement.Position < prize.Placement.Position)
            return -1;
        else if (Placement.Position > prize.Placement.Position)
            return 1;
        else if (Reward.GetType() == typeof(ExactPlacement) && prize.Reward.GetType() == typeof(GroupPlacement))
            return -1;
        else if (Reward.GetType() == typeof(GroupPlacement) && prize.Reward.GetType() == typeof(ExactPlacement))
            return 1;
        else
            return 0;
    }
}
