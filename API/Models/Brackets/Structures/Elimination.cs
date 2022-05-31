namespace API.Models.Brackets.Structures;

/// <summary>
/// A bracket component where the winner of the series progresses and the loser is eliminated
/// </summary>
public class Elimination : Structure
{
    public Elimination(Series series, IStructure child1, IPlacement placement1, IStructure child2, IPlacement placement2) : base(series)
    {
        child1.SetWinnerProgression(Series);
        child1.SetLoserProgression(placement1);

        child2.SetWinnerProgression(Series);
        child2.SetLoserProgression(placement2);
    }
}
