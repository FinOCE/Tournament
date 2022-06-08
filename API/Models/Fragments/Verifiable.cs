namespace API.Models.Fragments;

/// <summary>
/// A compositional fragment for models that can receive verifications
/// </summary>
public class Verifiable : IVerifiable
{
    public bool Verified { get; private set; }

    public void Verify()
    {
        Verified = true;
    }
}
