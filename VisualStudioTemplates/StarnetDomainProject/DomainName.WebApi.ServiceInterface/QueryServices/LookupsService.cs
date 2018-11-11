using $ext_projectname$.ReadModel;
using $ext_projectname$.WebApi.ServiceModel;
using ServiceStack;
using System.Threading.Tasks;

namespace $safeprojectname$.QueryServices
{
    public class LookupsService : Service
    {
        readonly IQueryById<Lookup> QueryById;

        public LookupsService(IQueryById<Lookup> queryById)
        {
            QueryById = queryById;
        }

        public async Task<object> Any(GetLookup req)
        {
            return await QueryById.GetById(req.Id);
        }
    }
}
