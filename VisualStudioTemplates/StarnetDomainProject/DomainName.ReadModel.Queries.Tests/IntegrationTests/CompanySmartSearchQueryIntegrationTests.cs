using NUnit.Framework;
using System.Threading.Tasks;

namespace $safeprojectname$.IntegrationTests
{
    class CompanySmartSearchQueryIntegrationTests : QueryIntegrationTestBase
    {
        [Test]
        public async Task CanExecute()
        {
            var qry = new CompanySmartSearchQuery(DocumentStore);
            var res = await qry.Execute(new SmartShearchQueryRequest { Qry = "*", CurrentPage = 0, PageSize = 1 });
        }
    }
}