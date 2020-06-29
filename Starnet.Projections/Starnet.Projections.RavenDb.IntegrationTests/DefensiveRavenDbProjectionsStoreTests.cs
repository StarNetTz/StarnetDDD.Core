using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Starnet.Projections.RavenDb.IntegrationTests
{
    public class DefensiveRavenDbProjectionsStoreTests : IntegrationTestBase
    {
        [Test]
        public async Task ShouldSaveAndLoadDocument()
        {
            var repo = new DefensiveRavenDbProjectionsStore(DocumentStore);
            var id = $"TestDocuments/{Guid.NewGuid()}";
            
            await repo.StoreAsync(new TestDocument { Id = id, SomeProp = "Hello1" });
            var doc = await repo.LoadAsync<TestDocument>(id);

            Assert.That(doc.Id, Is.EqualTo(id));
            Assert.That(doc.SomeProp, Is.EqualTo("Hello1"));
        }

        [Test]
        public async Task ShouldSaveAndLoadDocuments()
        {
            var repo = new DefensiveRavenDbProjectionsStore(DocumentStore);
            var id1 = $"TestDocuments/{Guid.NewGuid()}";
            var id2 = $"TestDocuments/{Guid.NewGuid()}";

            var doc1 = new TestDocument { Id = id1, SomeProp = "Hello1" };
            var doc2 = new TestDocument { Id = id2, SomeProp = "Hello2" };

            await repo.StoreInUnitOfWorkAsync(doc1, doc2);
            var docs = await repo.LoadAsync<TestDocument>(id1, id2);

            Assert.That(docs[id1].SomeProp, Is.EqualTo("Hello1"));
            Assert.That(docs[id2].SomeProp, Is.EqualTo("Hello2"));
        }
    }

    public class TestDocument
    {
        public string Id { get; set; }
        public string SomeProp { get; set; }
    }
}