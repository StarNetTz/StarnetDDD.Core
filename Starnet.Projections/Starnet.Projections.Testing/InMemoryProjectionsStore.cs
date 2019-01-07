using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Starnet.Projections.Testing
{
    public class InMemoryProjectionsStore : INoSqlStore, ISqlStore
    {
        ConcurrentDictionary<string, object> Store = new ConcurrentDictionary<string, object>();

        public Task<T> LoadAsync<T>(string id) where T : class
        {
            object returnValue = null;
            if (!Store.TryGetValue(id, out returnValue))
                return Task.FromResult(default(T));
            return Task.FromResult(returnValue as T);
        }

        public Task StoreAsync(object doc)
        {
            var id = doc.GetType().GetProperty("Id").GetValue(doc, null) as string;
            Store[id] = doc;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            object removedObj = null;
            Store.TryRemove(id, out removedObj);
            return Task.CompletedTask;
        }

        public Task StoreAsync<T>(T doc)
        {
            var id = doc.GetType().GetProperty("Id").GetValue(doc, null) as string;
            Store[id] = doc;
            return Task.CompletedTask;
        }

        public async Task StoreInUnitOfWorkAsync(params object[] docs)
        {
           foreach (var d in docs)
                await StoreAsync(d);
        }

        public async Task StoreInUnitOfWorkAsync<T>(params T[] docs)
        {
            foreach (var d in docs)
                await StoreAsync<T>(d);
        }
    }
}