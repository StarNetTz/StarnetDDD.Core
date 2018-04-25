using EventStore.ClientAPI;
using System;
using System.Linq;

namespace Starnet.Projections
{
    public interface IEventStoreConnectionFactory
    {
        IEventStoreConnection Create();
    }
}
