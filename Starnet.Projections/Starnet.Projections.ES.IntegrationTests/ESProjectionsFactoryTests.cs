using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

using System.Threading.Tasks;

namespace Starnet.Projections.ES.IntegrationTests
{
    [TestFixture]
    class ESProjectionsFactoryTests
    {
     
        IProjectionsFactory ProjectionsFactory;

        public ESProjectionsFactoryTests()
        {
            var services = new ServiceCollection();
            services.AddTransient<IHandlerFactory, StubHandlerFactory>();

            services.AddTransient<ICheckpointReader, StubCheckpointReader>();
            services.AddTransient<ICheckpointWriter, StubCheckpointWriter>();

            services.AddTransient<ISubscriptionFactory, ESSubscriptionFactory>();
            services.AddTransient<IProjectionsFactory, ProjectionsFactory>();
            var prov = services.BuildServiceProvider();

            ProjectionsFactory = prov.GetRequiredService<IProjectionsFactory>();
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