using NUnit.Framework;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.TestDriver;
using System.Threading.Tasks;

namespace $safeprojectname$.UnitTests
{
    class CompanySmartSearchQueryUnitTests : RavenTestDriver<MyRavenDBLocator>
    {
        IDocumentStore DocumentStore;

        public CompanySmartSearchQueryUnitTests()
        {
            DocumentStore = GetDocumentStore();
            CreateTestDocuments();
            WaitForIndexing(DocumentStore);
        }

            void CreateTestDocuments()
            {
                using (var s = DocumentStore.OpenSession())
                {
                    s.Store(new Company { Id = "Companies-1", Name = "Slime Ltd" });
                    s.Store(new Company { Id = "Companies-2", Name = "Blood Inc." });
                    s.SaveChanges();
                }
            }

        protected override void SetupDatabase(IDocumentStore documentStore)
        {
            base.SetupDatabase(documentStore);
            IndexCreation.CreateIndexes(typeof(Companies_Smart_Search).Assembly, documentStore);
        }

        [Test]
        public async Task CanExecute()
        {
            var qry = new CompanySmartSearchQuery(DocumentStore);
            var res = await qry.Execute(new SmartShearchQueryRequest { Qry = "*", CurrentPage = 0, PageSize = 10 });
            Assert.That(res.Data.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task OverflownQueryReturnsFirstPage()
        {
            var qry = new CompanySmartSearchQuery(DocumentStore);
            var res = await qry.Execute(new SmartShearchQueryRequest { Qry = "*", CurrentPage = 100, PageSize = 10 });
            Assert.That(res.Data.Count, Is.EqualTo(2));
        }      
    }
}