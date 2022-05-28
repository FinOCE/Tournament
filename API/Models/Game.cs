namespace API.Models;

public class Game
{
    public string Id { get; init; }
    public Series Series { get; init; }
    public Dictionary<string, int> Score { get; init; }
    public bool Finished { get; private set; }
    public string? Winner
    {
        get
        {
            if (!Finished)
                return null;

            return Score.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
    }

    public Game(string id, Series series)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        // Assign arguments to game
        Id = id;
        Series = series;
        Score = new();
        foreach (string teamId in Series.Teams.Keys)
            Score.Add(teamId, 0);
    }

    public Game(string id, Series series, Dictionary<string, int> score)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (series.Teams.Keys.Count != score.Keys.Count || !series.Teams.Keys.All(id => score.Keys.Contains(id)))
            throw new ArgumentException($"Invalid {nameof(series)} or {nameof(score)} provided");

        foreach (int s in score.Values)
            if (s < 0)
                throw new ArgumentException($"Invalid {nameof(score)} provided");

        // Assign arguments to game
        Id = id;
        Series = series;
        Score = score;
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
        if (!Finished && Score.ContainsKey(id))
        {
            if (score < 0)
                return false;

            Score[id] = score;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Finish the game
    /// </summary>
    public bool Finish()
    {
        // Prevent a tied game being finished
        if (Score.Values.ElementAt(0) == Score.Values.ElementAt(1))
            return false;

        Finished = true;
        return true;
    }
}
