using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Starnet.Projections
{
    public class ProjectionsFactory : IProjectionsFactory
    {
        protected readonly ICheckpointReader CheckpointReader;
        protected readonly ICheckpointWriterFactory CheckpointWriterFactory;
        protected readonly ISubscriptionFactory SubscriptionFactory;
        protected readonly IHandlerFactory HandlerFactory;
       

        public ProjectionsFactory(
            ICheckpointReader checkpointReader,
            ICheckpointWriterFactory checkpointWriterFactory,
            ISubscriptionFactory subscriptionFactory,
            IHandlerFactory denromalizerFactory
            )
        {
            CheckpointReader = checkpointReader;
            CheckpointWriterFactory = checkpointWriterFactory;
            SubscriptionFactory = subscriptionFactory;
            HandlerFactory = denromalizerFactory;
        
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
            var proj = Activator.CreateInstance(type) as Projection;
            proj.Name = GetCheckpointName(type);
            proj.Checkpoint = await CheckpointReader.Read($"Checkpoints-{proj.Name}");
            proj.CheckpointWriter = await CheckpointWriterFactory.Create();
            proj.Subscription = CreateSubscription(proj);
            proj.Handlers = GetHandlers(type);
         
            return proj;
        }

        private List<IHandler> GetHandlers(Type type)
        {

            Type[] typeArgs = (
                from iType in type.GetInterfaces()
                where iType.IsGenericType
                && iType.GetGenericTypeDefinition() == typeof(IHandledBy<>)
                select iType.GetGenericArguments()[0]).ToArray();
            var handlers = new List<IHandler>();
            foreach (var t in typeArgs)
                handlers.Add(HandlerFactory.Create(t));
            return handlers;
        }

        private string GetCheckpointName(Type type)
        {
            return type.Name.Replace("Projection", "");
        }

        private ISubscription CreateSubscription(IProjection proj)
        {
            var subscription = SubscriptionFactory.Create();
            subscription.StreamName = GetStreamName(proj.GetType());
            subscription.EventAppearedCallback = proj.Project;
            return subscription;
        }

        private string GetStreamName(Type type)
        {
            var attrInfo = type.GetCustomAttribute(typeof(SubscribesToStream)) as SubscribesToStream;
            return attrInfo.Name;
        }
    }
}