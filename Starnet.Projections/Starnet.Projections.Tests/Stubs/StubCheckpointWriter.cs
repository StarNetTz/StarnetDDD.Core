using System.Threading.Tasks;

namespace Starnet.Projections.Tests
{
    public class StubCheckpointWriter : ICheckpointWriter
    {
        public Task Write(Checkpoint checkpoint)
        {
            return Task.CompletedTask;
        }
    }
}