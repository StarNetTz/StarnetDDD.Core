using EventStore.ClientAPI;

namespace Starnet.Projections.ES
{
    public interface IESConnectionFactory
    {
        IEventStoreConnection Create();
    }
}
