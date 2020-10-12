using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Starnet.Projections.RavenDb.IntegrationTests
{
    class DefensiveRavenDbCheckpointReaderTests : IntegrationTestBase
    {

        [Test]
        public async Task CanWriteAndReadCheckpoint()
        {
            var w = new DefensiveRavenDbCheckpointWriter(DocumentStore);
            var r = new DefensiveRavenDbCheckpointReader(DocumentStore);
            var id = $"Checkpoints-{Guid.NewGuid()}";
            await w.Write(new Checkpoint { Id = id, Value = 1001 });
            var chp = await r.Read(id);
            Assert.That(chp.Value, Is.EqualTo(1001));
        }
    }
}
