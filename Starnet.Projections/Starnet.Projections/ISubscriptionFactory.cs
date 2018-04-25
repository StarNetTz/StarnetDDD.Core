using System;

namespace Starnet.Projections
{
    public interface ISubscriptionFactory
    {
        ISubscription Create();
    }
}
