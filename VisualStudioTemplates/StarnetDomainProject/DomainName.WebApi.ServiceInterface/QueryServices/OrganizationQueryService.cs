using $ext_projectname$.ReadModel;
using $ext_projectname$.WebApi.ServiceModel;
using ServiceStack;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace $safeprojectname$.QueryServices
{
    public class OrganizationQueryService : Service
    {
        readonly IOrganizationSmartSearchQuery Query;
        readonly IQueryById<Organization> QueryById;

        public OrganizationQueryService(IOrganizationSmartSearchQuery query, IQueryById<Organization> queryById)
        {
            Query = query;
            QueryById = queryById;
        }

        public async Task<object> Any(FindOrganizations req)
        {
            if (!string.IsNullOrEmpty(req.Id))
                return await GetById(req);
            return await PerformSmartSearch(req);
        }

        async Task<object> GetById(FindOrganizations req)
        {
            var c = await QueryById.GetById(req.Id);
            return c == null ? new PaginatedResult<Organization>() : new PaginatedResult<Organization>() { PageSize = 1, TotalItems = 1, CurrentPage = 0, TotalPages = 1, Data = new List<Organization>() { c } };
        }

        async Task<object> PerformSmartSearch(FindOrganizations req)
        {
            var smartSearchRequest = req.ConvertTo<SmartShearchQueryRequest>();
            var res = await Query.Execute(smartSearchRequest);
            return res;
        }
    }
}