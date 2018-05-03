using NUnit.Framework;
using System.Threading.Tasks;

namespace Starnet.Projections.Tests
{
    [TestFixture]
    class InMemorySubscriptionTests
    {
        InMemorySubscription Subscription;

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
            LastEvent = null;
            Subscription = new InMemorySubscription() { StreamName = "myStream", EventAppearedCallback = EventAppeared };
        }

        [Test]
        public async Task can_project()
        {
            Subscription.LoadEvents(new TestEvent() { Id = "1", SomeValue = "Manchester - Sloboda" });
            await Subscription.Start(0);
            AssertThatEventProjectedAsExpected();
        }

            void AssertThatEventProjectedAsExpected()
            {
                var e = LastEvent as TestEvent;
                Assert.That(Checkpoint == 1);
                Assert.That(e.Id == "1");
                Assert.That(e.SomeValue == "Manchester - Sloboda");
            }

        [Test]
        public async Task can_project_multiple_events()
        {
            LoadTwoEvents();
            await Subscription.Start(0);
            AssertLastEvent();
        }

        [Test]
        public async Task can_read_stream_from_given_checkpoint_of_2()
        {
            LoadTwoEvents();
            await Subscription.Start(2);
            AssertLastEvent();
        }

        void LoadTwoEvents()
        {
            Subscription.LoadEvents(
                new TestEvent() { Id = "1", SomeValue = "Manchester - Sloboda" },
                new TestEvent() { Id = "2", SomeValue = "Manchester - Sloboda City" }
                );
        }

        void AssertLastEvent()
        {
            var e = LastEvent as TestEvent;
            Assert.That(Checkpoint == 2);
            Assert.That(e.Id == "2");
            Assert.That(e.SomeValue == "Manchester - Sloboda City");
        }
    }
}