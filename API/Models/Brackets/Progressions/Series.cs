namespace API.Models.Brackets.Progressions;

public class Series : IProgression
{
    public string Id { get; init; }
    public Dictionary<string, ITeam> Teams { get; init; }
    public Dictionary<string, SeriesGame> Games { get; init; }
    public int BestOf { get; private set; }
    public IProgression? WinnerProgression { get; private set; }
    public IProgression? LoserProgression { get; private set; }
    public DateTime? StartedTimestamp { get; private set; }
    public bool Started { get { return StartedTimestamp != null; } }
    public DateTime? FinishedTimestamp { get; private set; }
    public bool Finished { get { return FinishedTimestamp != null; } }
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
                scores.Add(teamId, 0);

            foreach (SeriesGame game in Games.Values)
                if (game.Finished)
                    scores[game.Winner!]++;

            return scores;
        }
    }

    public Series(string id, Dictionary<string, ITeam>? teams, int bestOf)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (bestOf < 1)
            throw new ArgumentException($"Invalid {nameof(bestOf)} provided");

        // Assign arguments to series
        Id = id;
        Teams = teams ?? new();
        BestOf = bestOf;
        Games = new();
    }

    public Series(
        string id,
        Dictionary<string, ITeam> teams,
        int bestOf,
        Dictionary<string, SeriesGame> games,
        DateTime startedTimestamp,
        DateTime? finishedTimestamp,
        string? forfeiter)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (bestOf < 1)
            throw new ArgumentException($"Invalid {nameof(bestOf)} provided");

        foreach (SeriesGame game in games.Values)
            if (!teams.Keys.All(id => game.Score.ContainsKey(id)))
                throw new ArgumentException($"Invalid {nameof(teams)} or {nameof(games)} provided");

        if (forfeiter != null && !teams.ContainsKey(forfeiter))
            throw new ArgumentException($"Invalid {nameof(forfeiter)} provided");

        if (forfeiter != null && finishedTimestamp == null)
            throw new ArgumentException($"{nameof(finishedTimestamp)} cannot be null if {nameof(forfeiter)} is provided");

        // Assign arguments to series
        Id = id;
        Teams = teams;
        BestOf = bestOf;
        Games = games;
        StartedTimestamp = startedTimestamp;
        FinishedTimestamp = finishedTimestamp;
        Forfeiter = forfeiter;
    }

    public bool AddTeam(ITeam team)
    {
        if (Teams.Count == 2)
            return false;

        if (Teams.ContainsKey(team.Id))
            return false;

        Teams.Add(team.Id, team);
        return true;
    }

    public bool RemoveTeam(string id)
    {
        if (Started || Finished)
            return false;

        if (!Teams.ContainsKey(id))
            return false;

        Teams.Remove(id);
        return true;
    }

    /// <summary>
    /// Set how many games can be played at most
    /// </summary>
    public bool SetBestOf(int bestOf)
    {
        if (Started || Finished)
            return false;

        if (bestOf < 1)
            return false;

        BestOf = bestOf;
        return true;
    }

    /// <summary>
    /// Start the series
    /// </summary>
    public bool Start()
    {
        if (Teams.Count != 2)
            return false;

        StartedTimestamp = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Finish the series
    /// </summary>
    public bool Finish()
    {
        if (Finished)
            return false;

        Dictionary<string, int> score = Score;
        if (!Forfeited && score.Values.ElementAt(0) < (BestOf + 1) / 2 && score.Values.ElementAt(1) < (BestOf + 1) / 2)
            return false;

        FinishedTimestamp = DateTime.UtcNow;

        if (WinnerProgression != null)
            WinnerProgression.AddTeam(Teams[Winner!]);

        if (LoserProgression != null)
            LoserProgression.AddTeam(Teams.First(team => team.Key != Winner).Value);

        return true;
    }

    /// <summary>
    /// Make a team forfeit the series
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public bool Forfeit(string id)
    {
        if (!Teams.ContainsKey(id))
            return false;

        if (Finished)
            return false;

        Forfeiter = id;
        Finish();
        return true;
    }

    /// <summary>
    /// Set where the winner of the game should progress to
    /// </summary>
    public void SetWinnerProgression(IProgression? progression)
    {
        WinnerProgression = progression;
    }

    /// <summary>
    /// Set where the loser of the game should progress to
    /// </summary>
    public void SetLoserProgression(IProgression? progression)
    {
        LoserProgression = progression;
    }
}
