namespace Starnet.Projections.ES
{
    public class ESSubscriptionFactory : ISubscriptionFactory
    {
        readonly IESConnectionFactory EventStoreConnectionFactory;

        public ESSubscriptionFactory(IESConnectionFactory eventStoreConnectionFactory)
        {
            EventStoreConnectionFactory = eventStoreConnectionFactory;
        }

        public ISubscription Create()
        {
            return new ESSubscription(EventStoreConnectionFactory.Create());
        }
    }
}