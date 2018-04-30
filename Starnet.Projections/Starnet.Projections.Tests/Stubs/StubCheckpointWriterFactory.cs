using System.Threading.Tasks;

namespace Starnet.Projections.Tests
{
    public class StubCheckpointWriterFactory : ICheckpointWriterFactory
    {
        public Task<ICheckpointWriter> Create()
        {
            return Task.FromResult(new StubCheckpointWriter() as ICheckpointWriter);
        }
    }
}