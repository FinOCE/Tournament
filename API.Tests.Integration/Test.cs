using Microsoft.AspNetCore.Hosting;

namespace API.Tests.Integration;

public class Test
{
    protected readonly HttpClient _Client;
    protected readonly IConfiguration _Configuration;
    protected readonly DbService _Database;
    
    public Test()
    {
        Factory factory = new();
        _Client = factory.CreateClient();
        _Configuration = factory.GetConfiguration();
        _Database = (DbService)factory.Services.GetService(typeof(DbService))!;
    }

    private class Factory : WebApplicationFactory<Program>
    {
        protected IConfiguration? _Configuration { get; set; }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                _Configuration = conf.Build();
            });
            base.ConfigureWebHost(builder);
        }

        /// <summary>
        /// Get the configuration of the web application factory
        /// </summary>
        /// <returns>The configuration of the web application factory</returns>
        public IConfiguration GetConfiguration()
        {
            return _Configuration ?? new ConfigurationBuilder().Build();
        }
    }
}
