using $ext_projectname$.ReadModel.Projections;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using Starnet.Projections;
using Starnet.Projections.ES;
using System.Reflection;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    internal class ServiceInstance
    {
        readonly ILogger Logger;

        public ServiceInstance(ILogger logger)
        {
            Logger = logger;
        }

        public async Task Start(Container container)
        {
            var jsProjectionsFactory = container.GetInstance<JSProjectionsFactory>();
            await jsProjectionsFactory.CreateProjections();
            var projectionsFactory = container.GetInstance<ProjectionsFactory>();
            var projections = await projectionsFactory.Create(Assembly.GetAssembly(typeof(OrganizationProjection)));

            foreach (var p in projections)
            {
                Logger.LogInformation($"Starting {p.GetType().Name} on stream {p.Subscription.StreamName}.");
                await p.Start();
            }
        }

        public Task  Stop() {
            return Task.CompletedTask;
        }
    }
}