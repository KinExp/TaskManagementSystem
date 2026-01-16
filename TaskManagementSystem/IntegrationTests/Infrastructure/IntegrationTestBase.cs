using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.IntegrationTests.Infrastructure
{
    public abstract class IntegrationTestBase
        : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient _client;
        protected readonly CustomWebApplicationFactory _factory;

        protected IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        protected async Task ResetDatabaseAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Tasks.RemoveRange(db.Tasks);
            await db.SaveChangesAsync();
        }
    }
}
