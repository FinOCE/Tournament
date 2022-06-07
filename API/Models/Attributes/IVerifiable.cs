namespace API.Models.Attributes;

public interface IVerifiable
{
    bool Verified { get; }

    /// <summary>
    /// Verify this model
    /// </summary>
    public void Verify();
}
