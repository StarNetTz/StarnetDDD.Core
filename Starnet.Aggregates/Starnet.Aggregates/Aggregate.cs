using System.Collections.Generic;

namespace Starnet.Aggregates
{
    public abstract class Aggregate : IAggregate
    {
        private IAggregateState State;
        public List<object> Changes { get; private set; }

        public string Id
        {
            get
            {
                return State.Id;
            }
        }

        public int Version
        {
            get
            {
                return State.Version;
            }
        }

        public Aggregate(IAggregateState state)
        {
            State = state;
            Changes = new List<object>();
        }

        protected void Apply(object @event)
        {
            State.Mutate(@event);
            Changes.Add(@event);
        }
    }
}