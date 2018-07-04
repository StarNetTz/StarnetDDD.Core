using $ext_projectname$.ReadModel;
using $ext_projectname$.WebApi.ServiceInterface.QueryServices;
using $ext_projectname$.WebApi.ServiceModel;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using SimpleInjector;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class CompanyQueryServiceTests
    {
        private readonly ServiceStackHost AppHost;

        public CompanyQueryServiceTests()
        {
            AppHost = new BasicAppHost().Init();
            AppHost.Container.Adapter = new SimpleInjectorIocAdapter(SetupSimpleInjectorContainer());
        }

        static Container SetupSimpleInjectorContainer()
        {
            Container simpleContainer = new Container();
            simpleContainer.Register<ICompanySmartSearchQuery, StubCompanySmartSearchQuery>();
            simpleContainer.Register<CompanyQueryService>();
            simpleContainer.Register(typeof(IQueryById<>), typeof(StubQueryById<>));
            return simpleContainer;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => AppHost.Dispose();

        [Test]
        public async Task can_smart_search()
        {
            var service = AppHost.Container.Resolve<CompanyQueryService>();
            var response = await service.Any(new FindCompanies { CurrentPage = 0, PageSize = 10, Qry = "*" }) as PaginatedResult<Company>;
            Assert.That(response, Is.Null);
        }

        [Test]
        public async Task can_get_by_id()
        {
            var service = AppHost.Container.Resolve<CompanyQueryService>();
            var response = await service.Any(new FindCompanies { Id = "Companies-1" }) as PaginatedResult<Company>;
            Assert.That(response.Data, Is.Null);
        }

        class StubCompanySmartSearchQuery : ICompanySmartSearchQuery
        {
            public Task<PaginatedResult<Company>> Execute(ISmartSearchQueryRequest qry)
            {
                return Task.FromResult<PaginatedResult<Company>>(null);
            }
        }

        class StubQueryById<T> : IQueryById<T>
        {
            public Task<T> GetById(string id)
            {
                return Task.FromResult<T>(default(T));
            }
        }
    }
}