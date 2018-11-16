using NUnit.Framework;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.TestDriver;
using System.Threading.Tasks;

namespace $safeprojectname$.UnitTests
{
    class TypeaheadSmartSearchQueryUnitTests : RavenTestDriver
    {
        IDocumentStore DocumentStore;

        public TypeaheadSmartSearchQueryUnitTests()
        {
            DocumentStore = GetDocumentStore();
            CreateTestDocuments();
            WaitForIndexing(DocumentStore);
        }

            void CreateTestDocuments()
            {
                using (var s = DocumentStore.OpenSession())
                {
                    s.Store(new Organization { Id = "Organizations-1", Name = "Slime Ltd" });
                    s.Store(new Organization { Id = "Organizations-2", Name = "Blood Inc." });
                    s.SaveChanges();
                }
            }

        protected override  void SetupDatabase(IDocumentStore documentStore)
        {
            base.SetupDatabase(documentStore);
            IndexCreation.CreateIndexes(typeof(Organizations_Smart_Search).Assembly, documentStore);
        }

        [Test]
        public async Task CanExecute()
        {
            var qry = new TypeaheadSmartSearchQuery(DocumentStore);
            var res = await qry.Execute(new SmartShearchQueryRequest {Collection = TypeaheadSmartSearchQuery.OrganizationsCollectionName, Qry = "*", CurrentPage = 0, PageSize = 10 });
            Assert.That(res.Data.Count, Is.EqualTo(2));
        }


        [Test]
        public void unrecognized_collection_throws_argument_exception()
        {
            var qry = new TypeaheadSmartSearchQuery(DocumentStore);
            Assert.That(async ()=> { await qry.Execute(new SmartShearchQueryRequest { Collection = "unrecognized", Qry = "*", CurrentPage = 0, PageSize = 10 }); }, Throws.ArgumentException);
        }

        [Test]
        public async Task OverflownQueryReturnsFirstPage()
        {
            var qry = new OrganizationSmartSearchQuery(DocumentStore);
            var res = await qry.Execute(new SmartShearchQueryRequest { Qry = "*", CurrentPage = 100, PageSize = 10 });
            Assert.That(res.Data.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task CanGetOrganizationById()
        {
            var q = new QueryById<Organization>(DocumentStore);
            var cmp = await q.GetById("Organizations-1");
            Assert.That(cmp.Name, Is.EqualTo("Slime Ltd"));
        }
    }
}