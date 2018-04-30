using System;
using System.Threading.Tasks;

namespace Starnet.Projections.Tests
{
    public class StubCheckpointReader : ICheckpointReader
    {
        public Task<Checkpoint> Read(string id)
        {
            return Task.FromResult(new Checkpoint { Id = id, Value = 0 });
        }
    }

    public class StubHandlerFactory : IHandlerFactory
    {
        public IHandler Create(Type t)
        {
            return Activator.CreateInstance(t) as IHandler;
        }
    }

}
