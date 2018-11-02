using EventStore.ClientAPI;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Starnet.Projections.ES.IntegrationTests
{
    [TestFixture]
    class ESSubscriptionTests
    {
        ESSubscription Subscription;

        long Checkpoint = 0;
        object LastEvent = null;

        [SetUp]
        public void SetUp()
        {
            Checkpoint = 0;
            LastEvent = null;
            ConfigureSubscription();
        }

            void ConfigureSubscription()
            {
                Subscription = new ESSubscription(EventStoreConnection.Create(ESConnectionConfig.TcpEndpoint))
                {
                    StreamName = "$ce-Match",
                    EventAppearedCallback = EventAppeared
                };
                Subscription.Start(0).Wait();
            }

                Task EventAppeared(object ev, long checkpoint)
                {
                    Checkpoint = checkpoint;
                    LastEvent = ev;
                    return Task.CompletedTask;
                }

        [Test]
        public async Task can_write_events_to_event_store_and_project_them()
        {
            await new ESDataGenerator().WriteEventsToStore(2);
            await Task.Delay(500);
            AssertThatEventsProjected();
        }

            void AssertThatEventsProjected()
            {
                Assert.That(LastEvent, Is.InstanceOf<TestEvent>());
                Assert.That(LastEvent, Is.Not.Null);
                Assert.That((LastEvent as TestEvent).SomeValue, Contains.Substring("Match name:"));
                Assert.That(Checkpoint, Is.GreaterThan(0));
            }
    }
}