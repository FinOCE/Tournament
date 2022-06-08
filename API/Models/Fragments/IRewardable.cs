namespace API.Models.Fragments;

public interface IRewardable
{
    Dictionary<string, Prize> Prizes { get; }

    /// <summary>
    /// Set the prizes for the model
    /// </summary>
    /// <param name="prizes">The prizes to be added to the model</param>
    /// <exception cref="ArgumentException"></exception>
    void SetPrizes(Dictionary<string, Prize> prizes);

    /// <summary>
    /// Add a prize to the tournament
    /// </summary>
    /// <param name="prize">The prize to be added</param>
    /// <returns>Whether or not the prize was successfully added</returns>
    bool AddPrize(Prize prize);

    /// <summary>
    /// Removes a prize from the tournament
    /// </summary>
    /// <param name="id">The ID of the prize to be removed</param>
    /// <returns>Whether or not the prize was successfully removed</returns>
    bool RemovePrize(string id);
}
