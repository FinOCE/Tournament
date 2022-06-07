namespace API.Models.Attributes;

public class Verifiable : IVerifiable
{
    public bool Verified { get; private set; }

    public void Verify()
    {
        Verified = true;
    }
}
