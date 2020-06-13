using System;
using NUnit.Framework;
using System.Reflection;
using System.Threading.Tasks;
using Starnet.Projections.Testing;

using Microsoft.Extensions.DependencyInjection;
using Starnet.Projections.UnitTests;
using System.Collections.Generic;

namespace Starnet.Projections.Tests
{
    [TestFixture]
    class InMemoryProjectionsFactoryTests
    {
        IProjectionsFactory ProjectionsFactory;

        public InMemoryProjectionsFactoryTests()
        {
            var ServiceCollection = new ServiceCollection();

            ServiceCollection.AddSingleton<INoSqlStore, InMemoryProjectionsStore>();
            ServiceCollection.AddSingleton<ISqlStore, InMemoryProjectionsStore>();
            ServiceCollection.AddTransient<ICheckpointReader, StubCheckpointReader>();
            ServiceCollection.AddTransient<ICheckpointWriter, StubCheckpointWriter>();

            ServiceCollection.AddTransient<IHandlerFactory, DIHandlerFactory>();
            ServiceCollection.AddTransient<ISubscriptionFactory, InMemorySubscriptionFactory>();
            ServiceCollection.AddTransient<IProjectionsFactory, ProjectionsFactory>();
            ServiceCollection.AddTransient<ITimeProvider, MockTimeProvider>();
            ServiceCollection.AddTransient<FailingHandler>();
            ServiceCollection.AddTransient<TestHandler>();

            var provider = ServiceCollection.BuildServiceProvider();
            ProjectionsFactory = provider.GetRequiredService<IProjectionsFactory>();
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