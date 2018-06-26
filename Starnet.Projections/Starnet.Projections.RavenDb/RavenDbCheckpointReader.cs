using Raven.Client.Documents;
using System.Threading.Tasks;

namespace Starnet.Projections.RavenDb
{
    public class RavenDbCheckpointReader : ICheckpointReader
    {
        readonly IDocumentStore DocumentStore;

        public RavenDbCheckpointReader(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        public async Task<Checkpoint> Read(string id)
        {
            using (var s = DocumentStore.OpenAsyncSession())
            {
                var chp = await s.LoadAsync<Checkpoint>(id);
                if (null == chp)
                    chp = new Checkpoint { Id = id, Value = 0 };
                return chp;
            }
        }
    }
}
