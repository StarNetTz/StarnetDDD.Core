using NUnit.Framework;
using Raven.Client.Documents;
using Raven.TestDriver;
using System.Threading.Tasks;

namespace Starnet.Projections.RavenDb.IntegrationTests
{
    public class RavenDbProjectionsStoreTests : RavenTestDriver
    {
        RavenDbProjectionsStore Store;

        [OneTimeSetUp]
        public void Setup()
        {
            var store = GetDocumentStore();
            CreateTestDocuments(store);
            WaitForIndexing(store);
            Store = new RavenDbProjectionsStore(store);
        }

        protected override void SetupDatabase(IDocumentStore documentStore)
        {
            base.SetupDatabase(documentStore);
        }

        void CreateTestDocuments(IDocumentStore store)
        {
            using (var s = store.OpenSession())
            {
                s.Store(new Employee { Id = "1", FirstName = "John", LastName = "Doe" });
                s.Store(new Employee { Id = "2", FirstName = "Jane", LastName = "Doe" });
                s.SaveChanges();
            }
        }

        [Test]
        public async Task LoadsAsync()
        {
            var res = await Store.LoadAsync<Employee>("1");
            Assert.That(res.FirstName, Is.EqualTo("John"));
        }

        [Test]
        public async Task DeletesAsync()
        {
            await Store.StoreAsync(new Employee { Id = "0" });
            await Store.DeleteAsync("0");

            var res = await Store.LoadAsync<Employee>("0");

            Assert.That(res, Is.Null);
        }

        [Test]
        public async Task StoresAsync()
        {
            var obj = new Employee { Id = "3", FirstName = "A", LastName = "B" };
            await Store.StoreAsync((object)obj);

            var res = await Store.LoadAsync<Employee>("3");
            Assert.That(res.FirstName, Is.EqualTo("A"));
        }

        [Test]
        public async Task StoresAsyncUsingGenerics()
        {
            var obj = new Employee { Id = "4", FirstName = "A", LastName = "B" };
            await Store.StoreAsync<Employee>(obj);

            var res = await Store.LoadAsync<Employee>("4");
            Assert.That(res.FirstName, Is.EqualTo("A"));
        }

        [Test]
        public async Task StoresInUnitOfWorkAsync()
        {
            object obj1 = new Employee { Id = "5", FirstName = "A", LastName = "B" };
            object obj2 = new Employee { Id = "6", FirstName = "A", LastName = "B" };

            await Store.StoreInUnitOfWorkAsync(obj1, obj2);

            var res1 = await Store.LoadAsync<Employee>("5");
            var res2 = await Store.LoadAsync<Employee>("6");

            Assert.That(res1, Is.Not.Null);
            Assert.That(res2, Is.Not.Null);
        }

        [Test]
        public async Task StoresInUnitOfWorkAsyncUsingGenerics()
        {
            var obj1 = new Employee { Id = "7", FirstName = "A", LastName = "B" };
            var obj2 = new Employee { Id = "8", FirstName = "A", LastName = "B" };

            await Store.StoreInUnitOfWorkAsync<Employee>(obj1, obj2);

            var res1 = await Store.LoadAsync<Employee>("7");
            var res2 = await Store.LoadAsync<Employee>("8");

            Assert.That(res1, Is.Not.Null);
            Assert.That(res2, Is.Not.Null);
        }
    }
}
