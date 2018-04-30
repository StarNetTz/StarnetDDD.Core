using EventStore.ClientAPI;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Starnet.Projections.Tests
{
    [TestFixture]
    class GESSubscriptionTests
    {
        GESSubscription Subscription;

        long Checkpoint = 0;
        object LastEvent = null;

        private Task EventAppeared(object ev, long checkpoint)
        {
            Checkpoint = checkpoint;
            LastEvent = ev;
            return Task.CompletedTask;
        }

        [SetUp]
        public void SetUp()
        {
            Checkpoint = 0;
            LastEvent = null;
            ConfigureSubscription();
        }

        private void ConfigureSubscription()
        {
            Subscription = new GESSubscription(EventStoreConnection.Create(EventStoreConnectionSettings.TcpEndpoint));
            Subscription.StreamName = "$ce-Match";
            Subscription.EventAppearedCallback = EventAppeared;
            Subscription.Start(0).Wait();
        }

        [Test]
        public async Task can_write_events_to_event_store_and_project_them()
        {
            await new EventStoreTestDataGenerator().WriteEventsToStore(2);
            await Task.Delay(500);
            AssertThatEventsProjected();
        }

        private void AssertThatEventsProjected()
        {
            Assert.That(LastEvent, Is.InstanceOf<TestEvent>());
            Assert.That(LastEvent, Is.Not.Null);
            Assert.That((LastEvent as TestEvent).SomeValue, Contains.Substring("Match name:"));
            Assert.That(Checkpoint, Is.GreaterThan(0));
        }
    }
}