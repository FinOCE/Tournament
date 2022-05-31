namespace API.Models.Brackets.Structures;

public interface IStructure
{
    Series Series { get; }
    IStructure? Parent { get; }
    IStructure? Left { get; }
    IStructure? Right { get; }
    int Children { get; }

    /// <summary>
    /// Set where the winner progresses to
    /// </summary>
    void SetWinnerProgression(IProgression progression);

    /// <summary>
    /// Set where the loser progresses to
    /// </summary>
    void SetLoserProgression(IProgression progression);

    /// <summary>
    /// Add a child node to the structure
    /// </summary>
    void AddChild(IStructure child);
}
