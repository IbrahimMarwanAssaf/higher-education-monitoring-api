using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;

namespace UNIOOP.App.Tests.Integration.Infrastructure
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddUserSecrets<CustomWebApplicationFactory>();
            });

            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DataContextEF>();

            context.Database.EnsureDeleted();
            context.Database.Migrate();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var testPassword = configuration["IntegrationTestUsers:Password"];

            if (string.IsNullOrWhiteSpace(testPassword))
            {
                throw new InvalidOperationException("IntegrationTestUsers:Password is not configured.");
            }

            IntegrationTestSeeder.Seed(context, testPassword);

            return host;
        }
    }
}