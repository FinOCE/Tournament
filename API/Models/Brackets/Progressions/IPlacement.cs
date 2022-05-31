namespace API.Models.Brackets.Progressions;

public interface IPlacement : IProgression
{
    int Position { get; }
}
