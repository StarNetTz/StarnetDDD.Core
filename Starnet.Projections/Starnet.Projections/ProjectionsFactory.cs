using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Starnet.Projections
{
    public class ProjectionsFactory : IProjectionsFactory
    {
        readonly Container Container;

        public ProjectionsFactory(Container container)
        {
            Container = container;
        }

        public async Task<IList<IProjection>> Create(Assembly projectionsAssembly)
        {
            var type = typeof(IProjection);
            var ret = new List<IProjection>();
            var types = projectionsAssembly.GetTypes().Where(p => type.IsAssignableFrom(p)).ToList();
            foreach (var t in types)
                ret.Add(await Create(t));
            return ret;
        }

        public async Task<IProjection> Create<T>()
        {
            var type = typeof(T);
            return await Create(type);
        }

        public async Task<IProjection> Create(Type type)
        {
            var pi = GetProjectionInfo(type);
            var proj = new Projection();
            proj.Name = pi.Name;
            proj.SubscriptionStreamName = pi.SubscriptionStreamName;
            ICheckpointReader cr = Container.GetInstance<ICheckpointReader>();
            proj.Checkpoint = await cr.Read($"Checkpoints-{proj.Name}");

            ICheckpointWriter cw = Container.GetInstance<ICheckpointWriter>();
            proj.CheckpointWriter = cw;
            proj.Subscription = CreateSubscription(proj);
            proj.Handlers = GetHandlers(type);
            return proj;
        }

            ProjectionInfo GetProjectionInfo(Type type)
            {
                return new ProjectionInfo
                {
                    Name = GetProjectionName(type),
                    SubscriptionStreamName = GetSubscriptionStreamName(type)
                };
            }

                string GetProjectionName(Type type)
                {
                    return type.Name.Replace("Projection", "");
                }

                string GetSubscriptionStreamName(Type type)
                {
                    var attrInfo = type.GetCustomAttribute(typeof(SubscribesToStream)) as SubscribesToStream;
                    return attrInfo.Name;
                }

            ISubscription CreateSubscription(Projection proj)
            {
                var subscription = Container.GetInstance<ISubscriptionFactory>().Create();
                subscription.StreamName = proj.SubscriptionStreamName;
                subscription.EventAppearedCallback = proj.Project;
                return subscription;
            }

            List<IHandler> GetHandlers(Type type)
            {
                Type[] typeArgs = (
                    from iType in type.GetInterfaces()
                    where iType.IsGenericType
                    && iType.GetGenericTypeDefinition() == typeof(IHandledBy<>)
                    select iType.GetGenericArguments()[0]).ToArray();
                var handlers = new List<IHandler>();
                foreach (var t in typeArgs)
                    handlers.Add(Container.GetInstance<IHandlerFactory>().Create(t));
                return handlers;
            }

        class ProjectionInfo
        {
            public string Name { get; set; }
            public string SubscriptionStreamName { get; set; }
        }
    }
}