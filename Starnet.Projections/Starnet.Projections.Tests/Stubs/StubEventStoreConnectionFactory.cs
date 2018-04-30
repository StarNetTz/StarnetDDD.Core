using EventStore.ClientAPI;

namespace Starnet.Projections.Tests
{
    public class StubEventStoreConnectionFactory : IEventStoreConnectionFactory
    {
        public IEventStoreConnection Create()
        {
            var Connection = EventStoreConnection.Create(EventStoreConnectionSettings.TcpEndpoint);
            Connection.ConnectAsync().Wait();
            return Connection;
        }
    }
}