using $ext_projectname$.ReadModel;
using $ext_projectname$.WebApi.ServiceInterface.QueryServices;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using SimpleInjector;
using System.Threading.Tasks;

namespace $safeprojectname$.UnitTests.QueryServiceUnitTests
{
    class TypeaheadsServiceTests
    {
        private readonly ServiceStackHost AppHost;

        public TypeaheadsServiceTests()
        {
            AppHost = new BasicAppHost().Init();
            AppHost.Container.Adapter = new SimpleInjectorIocAdapter(SetupSimpleInjectorContainer());
        }

            static Container SetupSimpleInjectorContainer()
            {
                Container simpleContainer = new Container();
                simpleContainer.Register<ITypeaheadSmartSearchQuery, MockTypeaheadSmartSearchQuery>();
                simpleContainer.Register<TypeAheadQueryService>();
                return simpleContainer;
            }


        [Test]
        public async Task can_execute_request()
        {
            var service = AppHost.Container.Resolve<TypeAheadQueryService>();
            var req = new ServiceModel.FilterTypeahead();
            var response = await service.Any(req) as PaginatedResult<TypeaheadItem>;
            Assert.That(response, Is.Not.Null);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => AppHost.Dispose();
    }

    class MockTypeaheadSmartSearchQuery : ITypeaheadSmartSearchQuery
    {
        public Task<PaginatedResult<TypeaheadItem>> Execute(ISmartSearchQueryRequest qry)
        {
            return Task.FromResult(new PaginatedResult<TypeaheadItem>());
        }
    }


}