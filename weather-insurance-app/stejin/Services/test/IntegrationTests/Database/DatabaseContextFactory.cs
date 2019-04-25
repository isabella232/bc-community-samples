using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WeatherInsurance.Integration.Database;

namespace IntegrationTests.Database
{
    public class DatabaseContextFactory
    {

        public static Context CreateLiveContext()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configPath = AppContext.BaseDirectory;

            var builder = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlServer(config.GetConnectionString("Database"))
                .Options;

            var context = new Context(options);

            return context;
        }

        public static Context CreateMockContext(string testName)
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: testName)
                .Options;

            var context = new Context(options);
            return context;
        }
    }
}
