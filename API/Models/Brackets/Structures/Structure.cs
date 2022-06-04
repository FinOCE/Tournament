namespace API.Models.Brackets.Structures;

/// <summary>
/// The base class to extend structures from
/// </summary>
public class Structure : IStructure
{
    public Series Series { get; init; }
    public IStructure? Parent { get; protected set; } = null;
    public IStructure? Left { get; protected set; } = null;
    public IStructure? Right { get; protected set; } = null;
    public int Children
    {
        get
        {
            int children = 0;

            if (Left != null)
            {
                children++;
                children += Left.Children;
            }

            if (Right != null)
            {
                children++;
                children += Right.Children;
            }

            return children;
        }
    }

    public Structure(Series series)
    {
        Series = series;
    }

    public virtual void SetWinnerProgression(IProgression progression)
    {
        Series.SetWinnerProgression(progression);
    }

    public virtual void SetLoserProgression(IProgression progression)
    {
        Series.SetLoserProgression(progression);
    }

    public virtual IStructure? FindStructureWithTeam(string teamId)
    {
        if (Series.Teams.ContainsKey(teamId))
            return this;
        else return Left?.FindStructureWithTeam(teamId) ?? Right?.FindStructureWithTeam(teamId);
    }

    public virtual bool AddChild(IStructure child)
    {
        if (Left == null)
        {
            Left = child;
            return true;
        }
        else if (Right == null)
        {
            Right = child;
            return true;
        }
        else return false;
    }
}