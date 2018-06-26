using Raven.Client.Documents;
using System;
using System.Threading.Tasks;

namespace Starnet.Projections.RavenDb
{
    public class RavenDbProjectionsStore : INoSqlStore, ISqlStore
    {
        readonly IDocumentStore DocumentStore;

        public RavenDbProjectionsStore(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        public async Task<T> LoadAsync<T>(string id) where T : class
        {
            using (var s = DocumentStore.OpenAsyncSession())
                return await s.LoadAsync<T>(id);
        }

        public async Task StoreAsync(object doc)
        {
            using (var s = DocumentStore.OpenAsyncSession())
            {
                await s.StoreAsync(doc);
                await s.SaveChangesAsync();
            }
        }

        public Task DeleteAsync(string id)
        {
            using (var s = DocumentStore.OpenSession())
            {
                s.Delete(id);
                s.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public async Task StoreAsync<T>(T doc)
        {
            using (var s = DocumentStore.OpenAsyncSession())
            {
                await s.StoreAsync(doc);
                await s.SaveChangesAsync();
            }
        }
    }
}
