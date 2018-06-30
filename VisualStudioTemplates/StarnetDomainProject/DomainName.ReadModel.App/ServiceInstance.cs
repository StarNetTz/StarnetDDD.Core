using $ext_projectname$.ReadModel.Projections;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using Starnet.Projections;
using Starnet.Projections.ES;
using System.Reflection;

namespace $safeprojectname$
{
    internal class ServiceInstance
    {
        readonly ILogger Logger;

        public ServiceInstance(ILogger logger)
        {
            Logger = logger;
        }

        public void Start(Container container)
        {
            var jsProjectionsFactory = container.GetInstance<JSProjectionsFactory>();
            jsProjectionsFactory.CreateProjections().Wait();
            var projectionsFactory = container.GetInstance<ProjectionsFactory>();
            var projections = projectionsFactory.Create(Assembly.GetAssembly(typeof(CompanyProjection))).Result;

            foreach (var p in projections)
            {
                Logger.LogInformation($"Starting {p.GetType().Name} on stream {p.Subscription.StreamName}.");
                p.Start();
            }
        }

        public void Stop()
        {

        }
    }
}
