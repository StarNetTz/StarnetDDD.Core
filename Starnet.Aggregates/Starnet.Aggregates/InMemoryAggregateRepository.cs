using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Aggregates
{
    public class InMemoryAggregateRepository : IAggregateRepository
    {
        public class AggregateConcurrencyException : Exception
        {
            public readonly string Id;

            public AggregateConcurrencyException(string id)
                : base(string.Format("Concurrent update exception on aggregate: {0}", id))
            {
                Id = id;
            }
        }

        protected ConcurrentDictionary<string, List<object>> DataStore = new ConcurrentDictionary<string, List<object>>();

        private static void PerformConcurrencyCheck(IAggregate agg, List<object> events)
        {
            var originalVersion = agg.Version - agg.Changes.Count;
            if (events.Count != originalVersion)
                throw new AggregateConcurrencyException(agg.Id);
        }

        protected List<object> LoadEvents(string key)
        {
            return (!DataStore.ContainsKey(key)) ? new List<object>() : DataStore[key];
        }

        public Task<TAggregate> GetAsync<TAggregate>(string id) where TAggregate : class, IAggregate
        {
            return GetAsync<TAggregate>(id, Int32.MaxValue);
        }

        public Task<TAggregate> GetAsync<TAggregate>(string id, int version) where TAggregate : class, IAggregate
        {
            if (!DataStore.ContainsKey(id))
                return Task.FromResult(default(TAggregate));

            Type aggregateType = typeof(TAggregate);
            var state = AggregateStateFactory.CreateStateFor(aggregateType);

            var events = DataStore[id];
            foreach (var ev in events)
            {
                state.Mutate(ev);
                if (state.Version == version)
                    break;
            }
            var res = Activator.CreateInstance(aggregateType, state) as TAggregate;
            return Task.FromResult(res);
        }

        public virtual Task StoreAsync(IAggregate agg)
        {
            List<object> events = LoadEvents(agg.Id);
            PerformConcurrencyCheck(agg, events);
            events.AddRange(agg.Changes);
            DataStore[agg.Id] = events;
            agg.Changes.Clear();
            return Task.CompletedTask;
        }
    }
}