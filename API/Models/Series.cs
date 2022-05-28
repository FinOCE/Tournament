namespace API.Models;

public class Series
{
    public Dictionary<string, Team> Teams { get; init; }
    public Dictionary<string, Game> Games { get; init; }
    public int BestOf { get; init; }
    public bool Finished { get; private set; }
    public string? Forfeiter { get; private set; }
    public bool Forfeited { get { return Forfeiter != null; } }
    public string? Winner
    {
        get
        {
            if (!Finished)
                return null;

            if (Forfeited)
                return Teams.First(kvp => kvp.Key != Forfeiter).Key;

            return Score.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
    }
    public Dictionary<string, int> Score
    {
        get
        {
            Dictionary<string, int> scores = new();
            foreach (string teamId in Teams.Keys)
                Score.Add(teamId, 0);

            foreach (Game game in Games.Values)
                if (game.Finished)
                    scores[game.Winner!]++;

            return scores;
        }
    }

    public Series(string id, Dictionary<string, Team> teams, int bestOf)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (bestOf < 1)
            throw new ArgumentException($"Invalid {nameof(bestOf)} provided");

        // Assign arguments to series
        Teams = teams;
        BestOf = bestOf;
        Games = new();
    }

    public Series(string id, Dictionary<string, Team> teams, int bestOf, Dictionary<string, Game> games)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (bestOf < 1)
            throw new ArgumentException($"Invalid {nameof(bestOf)} provided");

        foreach (Game game in games.Values)
            if (!teams.Keys.Equals(game.Score.Keys))
                throw new ArgumentException($"Invalid {nameof(teams)} or {nameof(games)} provided");

        // Assign arguments to series
        Teams = teams;
        BestOf = bestOf;
        Games = games;
    }

    /// <summary>
    /// Finish the series
    /// </summary>
    public bool Finish()
    {
        Dictionary<string, int> score = Score;

        if (score.Values.ElementAt(0) == score.Values.ElementAt(1))
            return false;

        Finished = true;
        return true;
    }

    /// <summary>
    /// Make a team forfeit the series
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public void Forfeit(string id)
    {
        if (!Teams.ContainsKey(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        Forfeiter = id;
        Finish();
    }
}
