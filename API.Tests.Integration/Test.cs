namespace API.Tests.Integration;

public class Test
{
    protected readonly HttpClient _Client;
    protected readonly DbService _Database;
    
    public Test()
    {
        var factory = new WebApplicationFactory<Program>();
        _Client = factory.CreateClient();
        _Database = (DbService)factory.Services.GetService(typeof(DbService))!;
    }
}
