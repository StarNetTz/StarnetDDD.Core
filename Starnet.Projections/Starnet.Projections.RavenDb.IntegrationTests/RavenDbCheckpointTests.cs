using NUnit.Framework;
using Raven.Client.Documents;
using Raven.TestDriver;
using System.Threading.Tasks;

namespace Starnet.Projections.RavenDb.IntegrationTests
{
    class RavenDbCheckpointTests : RavenTestDriver
    {
        RavenDbCheckpointWriter Writer;
        RavenDbCheckpointReader Reader;

        const string NonExistantCheckpointId = "0";

        [OneTimeSetUp]
        public void Setup()
        {
            var store = GetDocumentStore();
            WaitForIndexing(store);
            Writer = new RavenDbCheckpointWriter(store);
            Reader = new RavenDbCheckpointReader(store);
        }

        protected override void SetupDatabase(IDocumentStore documentStore)
        {
            base.SetupDatabase(documentStore);
        }

        [Test]
        public async Task WritesAndReadsCheckpoint()
        {
            var checkpoint = new Checkpoint { Id = "1", Value = 1L };

            await Writer.Write(checkpoint);

            var res = await Reader.Read("1");
            Assert.That(res, Is.Not.Null);
        }

        [Test]
        public async Task ReturnsNewCheckpointIfNotFound()
        {
            var res = await Reader.Read(NonExistantCheckpointId);

            Assert.That(res.Value, Is.EqualTo(0));
        }
    }
}
