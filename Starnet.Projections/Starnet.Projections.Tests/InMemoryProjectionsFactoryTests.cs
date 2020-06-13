using System;
using NUnit.Framework;
using SimpleInjector;
using System.Reflection;
using System.Threading.Tasks;
using Starnet.Projections.Testing;
using Starnet.Projections.UnitTests;

namespace Starnet.Projections.Tests
{
    [TestFixture]
    class InMemoryProjectionsFactoryTests
    {
        readonly Container Container;
        IProjectionsFactory ProjectionsFactory;

        public InMemoryProjectionsFactoryTests()
        {
            Container = new Container();
            var svcProviderInstance = new SimpleInjectorServiceProvider() {  Container = Container} ;
            Container.RegisterInstance<IServiceProvider>(svcProviderInstance);

            Container.Register<IHandlerFactory, DIHandlerFactory>();
            Container.Register<ICheckpointReader, StubCheckpointReader>();
            Container.Register<ICheckpointWriterFactory, StubCheckpointWriterFactory>();
            Container.Register<ICheckpointWriter, StubCheckpointWriter>();
            Container.Register<ISubscriptionFactory, InMemorySubscriptionFactory>();
            Container.Register<INoSqlStore, InMemoryProjectionsStore>();
          
            Container.Register<IProjectionsFactory, ProjectionsFactory>();
            Container.Register<ITimeProvider, MockTimeProvider>();
            Container.Register<FailingHandler>();
            Container.Register<TestHandler>();
            Container.Verify();

            ProjectionsFactory = Container.GetInstance<IProjectionsFactory>();
        }



        [Test]
        public async Task can_create_test_projection_and_project()
        {
            var proj = await ProjectionsFactory.Create<TestProjection>();
            PreloadProjectionsSubscription(proj);

            await proj.Start();

            Assert.That(proj.Checkpoint.Value, Is.EqualTo(2));
        }

            void PreloadProjectionsSubscription(IProjection proj)
            {
                var sub = proj.Subscription as InMemorySubscription;
                sub.LoadEvents(
                   new TestEvent() { Id = "1", SomeValue = "Manchester - Sloboda" },
                   new TestEvent() { Id = "2", SomeValue = "Manchester - Sloboda City" }
                   );
            }

        [Test]
        public async Task failing_projection_throws_an_aggregate_exception()
        {
            var proj = await ProjectionsFactory.Create<FailingProjection>();
            PreloadFailingProjectionsSubscription(proj);
            Assert.That(async () => { await proj.Start(); }, Throws.Exception.TypeOf<AggregateException>());
        }

        void PreloadFailingProjectionsSubscription(IProjection proj)
            {
                var sub = proj.Subscription as InMemorySubscription;
                sub.LoadEvents(
                   new TestEvent() { Id = "1", SomeValue = "Manchester - Sloboda" },
                   new TestEvent() { Id = "2", SomeValue = "Manchester - Sloboda City" }
                   );
            }

        [Test]
        public async Task can_create_all_projections_within_assembly()
        {
            var projections = await ProjectionsFactory.Create(Assembly.GetAssembly(typeof(TestProjection)));
            Assert.That(projections.Count, Is.EqualTo(2));
        }
    }
}