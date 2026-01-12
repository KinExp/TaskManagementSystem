using System.Net.Http;

namespace TaskManagement.IntegrationTests.Infrastructure
{
    public class IntegrationTestBase
        : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient _client;

        public IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
    }
}
