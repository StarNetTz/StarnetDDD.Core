using NUnit.Framework;
using SimpleInjector;
using System;
using System.Threading.Tasks;

namespace Starnet.Projections.Testing
{
    public class ProjectionSpecification<TProjection>
      where TProjection : class, IProjection
    {
        public Container Container { get; set; }

        public IProjectionsStore ProjectionsStore { get; set; }
        ProjectionsFactory ProjectionsFactory;

        protected virtual void ConfigureContainer(Container container) { }

        public ProjectionSpecification()
        {
            Container = new Container();
            Container.Register<INoSqlStore, InMemoryProjectionsStore>(Lifestyle.Singleton);
            Container.Register<ISqlStore, InMemoryProjectionsStore>(Lifestyle.Singleton);
            Container.Register<ICheckpointReader, StubCheckpointReader>();
            Container.Register<ICheckpointWriter, StubCheckpointWriter>();
            Container.Register<IHandlerFactory, DIHandlerFactory>();
            Container.Register<ISubscriptionFactory, InMemorySubscriptionFactory>();
            Container.Register<IProjectionsFactory, ProjectionsFactory>();
            ConfigureContainer(Container);
            ProjectionsFactory = Container.GetInstance<ProjectionsFactory>();
            ProjectionsStore = Container.GetInstance<INoSqlStore>();
            Container.Verify();
        }

        public async Task Given(params object[] args)
        {
            var p = await ProjectionsFactory.Create<TProjection>();
            var s = p.Subscription as InMemorySubscription;
            s.LoadEvents(args);
            await p.Start();
        }

        public async Task Expect<TModel>(TModel model) where TModel : class
        {
            var id = ExtractIdFromObject(model);
            var actual = await ProjectionsStore.LoadAsync<TModel>(id);
            var diff = ObjectComparer.FindDifferences(model, actual);
            if (!string.IsNullOrEmpty(diff))
                throw new AssertionException(diff);
        }

        private static string ExtractIdFromObject(object model)
        {
            var id = model.GetType().GetProperty("Id").GetValue(model, null);
            ValidateIdType(id);
            return id.ToString();
        }

        static void ValidateIdType(object id)
        {
            switch (id)
            {
                case string s:
                case int i:
                case long l:
                case Guid g:
                    return;
                default:
                    throw new ArgumentException("Unsopported Id type!");
            }
        }
    }
}