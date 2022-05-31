namespace API.Models.Brackets.Structures;

/// <summary>
/// A bracket component for the end of the tournament
/// </summary>
public class Finale : Structure
{
    public Finale(Series series, ExactPlacement winner, ExactPlacement loser) : base(series)
    {
        base.SetWinnerProgression(winner);
        base.SetLoserProgression(loser);
    }

    public override void SetWinnerProgression(IProgression progression) { }

    public override void SetLoserProgression(IProgression progression) { }
}
