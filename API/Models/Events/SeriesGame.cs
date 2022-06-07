namespace API.Models.Events;

public class SeriesGame
{
    public string Id { get; init; }
    public string Name { get; init; }
    public Series Series { get; init; }
    public Dictionary<string, int> Score { get; init; }
    public bool Finished { get { return FinishedTimestamp is not null; } }
    public DateTime? FinishedTimestamp { get; private set; }
    public string? Winner
    {
        get
        {
            if (!Finished)
                return null;

            return Score.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
    }

    /// <exception cref="ArgumentException"></exception>
    public SeriesGame(
        string id,
        Series series,
        Dictionary<string, int>? score = null,
        DateTime? finishedTimestamp = null)
    {
        // Validate
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (score is not null)
        {
            if (series.Teams.Keys.Count != score.Keys.Count || !series.Teams.Keys.All(id => score.ContainsKey(id)))
                throw new ArgumentException($"Invalid {nameof(series)} or {nameof(score)} provided");

            foreach (int s in score.Values)
                if (s < 0)
                    throw new ArgumentException($"Invalid {nameof(score)} provided");

            if (finishedTimestamp is not null && score.Values.All(s => s.Equals(score.Values.First())))
                throw new ArgumentException($"Game with a tied score cannot have a timestamp for finishing");
        }
        else
        {
            score = new();
            foreach (string teamId in series.Teams.Keys)
                score.Add(teamId, 0);

            if (finishedTimestamp is not null)
                throw new ArgumentException($"Game with no score cannot have a timestamp for finishing");
        }


        // Instantiate
        Id = id;
        Name = $"Game {series.Games.Keys.Where(gameId => ulong.Parse(gameId) < ulong.Parse(id)).ToArray().Length + 1}";
        Series = series;
        Score = score;
        FinishedTimestamp = finishedTimestamp;

        Series.Games.Add(Id, this);
    }

    /// <summary>
    /// Get a team's current score
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public int GetScore(string id)
    {
        if (!Score.ContainsKey(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        return Score[id];
    }

    /// <summary>
    /// Add a point to the given team
    /// </summary>
    public bool SetScore(string id, int score)
    {
        if (Finished || !Score.ContainsKey(id))
            return false;

        if (score < 0)
            return false;

        Score[id] = score;
        return true;
    }

    /// <summary>
    /// Finish the game
    /// </summary>
    public bool Finish()
    {
        // Prevent a tied game being finished
        if (Score.Values.ElementAt(0) == Score.Values.ElementAt(1))
            return false;

        FinishedTimestamp = DateTime.UtcNow;
        return true;
    }
}
