using System.Collections.Generic;

namespace Starnet.Aggregates
{
    public interface IAggregate
    {
        string Id { get; }

        int Version { get; }

        List<object> Changes { get; }
        List<object> PublishedEvents { get; }
    }
}