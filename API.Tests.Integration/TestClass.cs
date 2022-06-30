namespace API.Tests.Integration;

public class TestClass
{
    protected readonly HttpClient Client;

    public TestClass()
    {
        Client = new Application().CreateClient();
    }
}
