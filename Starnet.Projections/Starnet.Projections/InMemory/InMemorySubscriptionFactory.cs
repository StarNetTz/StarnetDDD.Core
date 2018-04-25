using System;

namespace Starnet.Projections
{
    public class InMemorySubscriptionFactory : ISubscriptionFactory
    {
        public ISubscription Create()
        {
            return new InMemorySubscription();
        }
    }
}
