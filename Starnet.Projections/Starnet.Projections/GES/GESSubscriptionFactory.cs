using System;

namespace Starnet.Projections
{
    public class GESSubscriptionFactory : ISubscriptionFactory
    {
        readonly IEventStoreConnectionFactory EventStoreConnectionFactory;

        public GESSubscriptionFactory(IEventStoreConnectionFactory eventStoreConnectionFactory)
        {
            EventStoreConnectionFactory = eventStoreConnectionFactory;
        }
        public ISubscription Create()
        {
            return new GESSubscription(EventStoreConnectionFactory.Create());
        }
    }
}
