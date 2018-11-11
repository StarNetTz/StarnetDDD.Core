using $ext_projectname$.ReadModel;
using $ext_projectname$.WebApi.ServiceInterface.QueryServices;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using SimpleInjector;
using System.Threading.Tasks;

namespace $safeprojectname$.UnitTests.QueryServiceUnitTests
{
    class LookupsServiceTests
    {
        private readonly ServiceStackHost AppHost;

        public LookupsServiceTests()
        {
            AppHost = new BasicAppHost().Init();
            AppHost.Container.Adapter = new SimpleInjectorIocAdapter(SetupSimpleInjectorContainer());
        }

            static Container SetupSimpleInjectorContainer()
            {
                Container simpleContainer = new Container();
                simpleContainer.Register<LookupsService>();
                simpleContainer.Register<IQueryById<Lookup>, LookupQueryById>();
                return simpleContainer;
            }

        [Test]
        public async Task can_execute_request()
        {
            var service = AppHost.Container.Resolve<LookupsService>();
            var req = new ServiceModel.GetLookup();
            var response = await service.Any(req) as Lookup;
            Assert.That(response, Is.Not.Null);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => AppHost.Dispose();
    }

    class LookupQueryById : IQueryById<Lookup>
    {
        public Task<Lookup> GetById(string id)
        {
            var c = new Lookup() {Id = id, Data = new System.Collections.Generic.List<LookupItem> { new LookupItem { Id = "Items-1", Value = "Item 1" } } };
            return Task.FromResult(c);
        }
    }
}