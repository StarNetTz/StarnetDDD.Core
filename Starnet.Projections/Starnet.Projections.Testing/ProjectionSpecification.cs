using NUnit.Framework;
using SimpleInjector;
using System;
using System.Threading.Tasks;

namespace Starnet.Projections.Testing
{
    public class ProjectionSpecification<T, TModel>
       where T : class, IProjection
       where TModel : class
    {
        public Container Container { get; set; }

        public IProjectionsStore ProjectionsStore { get; set; }
        ProjectionsFactory ProjectionsFactory;

        protected virtual void ConfigureContainer(Container container) {}

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
            var p = await ProjectionsFactory.Create<T>();
            var s = p.Subscription as InMemorySubscription;
            s.LoadEvents(args);
            await p.Start();
        }

        public async Task Expect(object model)
        {
            var id = ExtractIdFromObject(model);
            var actual = await ProjectionsStore.LoadAsync<TModel>(id);
            var diff = ObjectComparer.FindDifferences(model, actual);
            if (!string.IsNullOrEmpty(diff))
                throw new AssertionException(diff);
        }

        private static string ExtractIdFromObject(object model)
        {
            return model.GetType().GetProperty("Id").GetValue(model, null) as string;
        }
    }
}
