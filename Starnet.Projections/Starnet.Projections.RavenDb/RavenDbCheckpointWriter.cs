using Raven.Client.Documents;
using System.Threading.Tasks;

namespace Starnet.Projections.RavenDb
{
    public class RavenDbCheckpointWriter : ICheckpointWriter
    {
        readonly IDocumentStore DocumentStore;

        public RavenDbCheckpointWriter(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        public async Task Write(Checkpoint checkpoint)
        {
            using (var s = DocumentStore.OpenAsyncSession())
            {
                await s.StoreAsync(checkpoint);
                await s.SaveChangesAsync();
            }
        }
    }
}