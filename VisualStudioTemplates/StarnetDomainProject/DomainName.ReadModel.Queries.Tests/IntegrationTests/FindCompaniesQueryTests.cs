using NUnit.Framework;
using System.Threading.Tasks;

namespace $safeprojectname$.IntegrationTests
{
    class FindCompaniesQueryIntegrationTests : QueryIntegrationTestBase
    {
        [Test]
        public async Task CanExecute()
        {
            var qry = new FindCompaniesQuery(DocumentStore);
            var res = await qry.Execute(new SmartShearchQueryRequest { Qry = "*", CurrentPage = 0, PageSize = 1 });
        }
    }
}