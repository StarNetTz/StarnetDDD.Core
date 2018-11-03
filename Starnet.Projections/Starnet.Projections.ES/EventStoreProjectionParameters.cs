using System;
using System.Collections.Generic;

namespace Starnet.Projections.ES
{
    public class EventStoreProjectionParameters
    {
        public string Name { get; set; }
        public List<string> SourceStreamNames { get; set; }
        public string DestinationStreamName { get; set; }
        public Type[] EventsToInclude { get; set; }
    }
}
