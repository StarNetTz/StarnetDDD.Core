using NUnit.Framework;
using SimpleInjector;
using System.Threading.Tasks;

namespace Starnet.Projections.Tests
{
    [TestFixture]
    class GESProjectionsFactoryTests
    {
        readonly Container Container;
        ProjectionsFactory ProjectionsFactory;

        public GESProjectionsFactoryTests()
        {
            Container = new Container();

            Container.Register<IHandlerFactory, StubHandlerFactory>();
            Container.Register<IFailureNotifierFactory, StubFailureNotifierFactory>();
            Container.Register<ICheckpointReader, StubCheckpointReader>();
            Container.Register<ICheckpointWriterFactory, StubCheckpointWriterFactory>();
            Container.Register<IEventStoreConnectionFactory>(() => new StubEventStoreConnectionFactory());
            Container.Register<ISubscriptionFactory, GESSubscriptionFactory>();
            Container.Register<IProjectionsFactory, ProjectionsFactory>();
            Container.Verify();
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ProjectionsFactory = Container.GetInstance<ProjectionsFactory>();
        }

        [Test]
        public async Task can_create_test_projection_and_project()
        {
            var proj = await ProjectionsFactory.Create<TestProjection>();
            await PreloadProjectionsSubscription();

            await proj.Start();
            await Task.Delay(500);

            Assert.That(proj.Checkpoint.Value, Is.EqualTo(2));
        }

        private async Task PreloadProjectionsSubscription()
        {
            await new EventStoreTestDataGenerator().WriteEventsToStore(2);
        }
    }
}