namespace API.Models.Brackets.Structures;

public interface IStructure
{
    Series Series { get; }
    IStructure? Parent { get; }
    IStructure? Left { get; }
    IStructure? Right { get; }
    IStructure[] Children { get; }

    /// <summary>
    /// Set where the winner progresses to
    /// </summary>
    void SetWinnerProgression(IProgression progression);

    /// <summary>
    /// Set where the loser progresses to
    /// </summary>
    void SetLoserProgression(IProgression progression);

    /// <summary>
    /// Find the nested structure that has the given team in it
    /// </summary>
    IStructure? FindStructureWithTeam(string teamId);

    /// <summary>
    /// Add a child node to the structure
    /// </summary>
    bool AddChild(IStructure child);
}
