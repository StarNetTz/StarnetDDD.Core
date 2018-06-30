using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace $safeprojectname$.IntegrationTests
{
    class RavenDbIntegrationTests
    {
        [Test]
        public void CanConnectToServer()
        {
            var conf = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            var rconf = RavenConfig.FromConfiguration(conf);
            var store = new RavenDocumentStoreFactory().CreateDocumentStore(rconf); 
            using (var s = store.OpenSession()) {}
        }
    }
}
