using $ext_projectname$.ReadModel;
using $ext_projectname$.WebApi.ServiceModel;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.QueryServices
{
    public class CompanyQueryService : Service
    {
        readonly IFindCompaniesQuery Query;

        public CompanyQueryService(IFindCompaniesQuery query)
        {
            Query = query;
        }

        public async Task<object> Any(FindCompanies request)
        {
            var req = request.ConvertTo<SmartShearchQueryRequest>();
            return await Query.Execute(req);
        }
    }

}
