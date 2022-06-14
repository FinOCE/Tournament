namespace API.Models.Socials;

/// <summary>
/// An invite to an exclusive event
/// </summary>
public class BracketInvite
{
    public string Id { get; init; }
    public IBracketBuilder Builder { get; init; }
    public ITeam Team { get; init; }
    public int Seed { get; init; }
    public bool Accepted { get; private set; }
    public bool Declined { get; private set; }

    /// <summary>
    /// Instantiate an invite for a team to an event
    /// </summary>
    /// <param name="id">The ID of the invite</param>
    /// <param name="builder">The builder for the bracket to be invited to</param>
    /// <param name="team">The team to invite</param>
    /// <param name="seed">The seed to place the team in</param>
    /// <exception cref="ArgumentException"></exception>
    public BracketInvite(
        string id,
        IBracketBuilder builder,
        ITeam team,
        int seed = BracketBuilder.DefaultSeed)
    {
        // Validate
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid ${id} provided");

        // Instantiate
        Id = id;
        Builder = builder;
        Team = team;
        Seed = seed;
    }

    /// <summary>
    /// Accept the invite
    /// </summary>
    /// <returns>Whether or not the invite was successfully accepted</returns>
    public bool Accept()
    {
        if (Accepted || Declined)
            return false;

        Accepted = Builder.AddTeam(Team, Seed);
        return Accepted;
    }

    /// <summary>
    /// Decline the invite
    /// </summary>
    /// <returns>Whether or not the invite was successfully declined</returns>
    public bool Decline()
    {
        if (Accepted || Declined)
            return false;

        Declined = true;
        return true;
    }
}
