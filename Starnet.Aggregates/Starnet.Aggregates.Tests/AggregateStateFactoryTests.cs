using NUnit.Framework;
using Starnet.SampleDomain;

namespace Starnet.Aggregates.Tests
{
    class AggregateStateFactoryTests
    {
       
        [Test]
        public void aggregate_state_factory_given_aggregate_creates_state()
        {
            var state = AggregateStateFactory.CreateStateFor(typeof(PersonAggregate));
            Assert.That(state is PersonAggregateState);
        }
    }
}