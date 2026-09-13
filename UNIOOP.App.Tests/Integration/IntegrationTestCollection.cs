using Xunit;
using UNIOOP.App.Tests.Integration.Infrastructure;

namespace UNIOOP.App.Tests.Integration
{
    [CollectionDefinition("Integration Tests")]
    public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
    }
}