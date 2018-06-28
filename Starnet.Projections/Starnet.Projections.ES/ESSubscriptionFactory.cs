using EventStore.ClientAPI;

namespace Starnet.Projections.ES
{
    public class ESSubscriptionFactory : ISubscriptionFactory
    {
        public ISubscription Create()
        {
            return new ESSubscription(EventStoreConnection.Create(ESConnectionConfig.ConnectionString));
        }
    }
}