namespace API.Models.Fragments;

/// <summary>
/// A compositional fragment for models that earn rewards
/// </summary>
public class Rewardable : IRewardable
{
    public Dictionary<string, Prize> Prizes { get; private set; } = new();

    public void SetPrizes(Dictionary<string, Prize> prizes)
    {
        foreach (KeyValuePair<string, Prize> kvp in prizes)
            if (!kvp.Key.Equals(kvp.Value.Id))
                throw new ArgumentException($"Invalid {nameof(prizes)} provided");

        Prizes = prizes;
    }

    public bool AddPrize(Prize prize)
    {
        if (Prizes.ContainsKey(prize.Id))
            return false;

        Prizes.Add(prize.Id, prize);
        return true;
    }

    public bool RemovePrize(string id)
    {
        if (!Prizes.ContainsKey(id))
            return false;

        Prizes.Remove(id);
        return true;
    }
}
