using System;
using System.Threading.Tasks;

namespace Starnet.Projections.Testing
{
    public class StubCheckpointReader : ICheckpointReader
    {
        public Task<Checkpoint> Read(string id)
        {
            return Task.FromResult(new Checkpoint { Id = id, Value = 0 });
        }
    }
}
