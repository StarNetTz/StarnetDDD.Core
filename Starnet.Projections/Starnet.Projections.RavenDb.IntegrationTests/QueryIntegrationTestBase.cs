using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;

namespace Starnet.Projections.RavenDb.IntegrationTests
{
    public class IntegrationTestBase
    {
        internal readonly IDocumentStore DocumentStore;

        public IntegrationTestBase()
        {
            var conf = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            var rconf = RavenConfig.FromConfiguration(conf);
            DocumentStore = new RavenDocumentStoreFactory().CreateAndInitializeDocumentStore(rconf);
        }
    }
}
