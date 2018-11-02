using NUnit.Framework;
using SimpleInjector;
using System;
using System.Threading.Tasks;

namespace Starnet.Projections.ES.IntegrationTests
{
    [TestFixture]
    class ESProjectionsFactoryTests
    {
        readonly Container Container;
        ProjectionsFactory ProjectionsFactory;

        public ESProjectionsFactoryTests()
        {
            Container = new Container();

            Container.Register<IHandlerFactory, StubHandlerFactory>();

            Container.Register<ICheckpointReader, StubCheckpointReader>();
            Container.Register<ICheckpointWriter, StubCheckpointWriter>();
           
            Container.Register<ISubscriptionFactory, ESSubscriptionFactory>();
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

            Assert.That(proj.Checkpoint.Value, Is.GreaterThan(0));
        }

        async Task PreloadProjectionsSubscription()
        {
            await new ESDataGenerator().WriteEventsToStore(2);
        }
    }
}
