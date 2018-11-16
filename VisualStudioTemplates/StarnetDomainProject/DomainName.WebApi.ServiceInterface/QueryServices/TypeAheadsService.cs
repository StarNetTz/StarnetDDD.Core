using $ext_projectname$.ReadModel;
using $ext_projectname$.WebApi.ServiceModel;
using ServiceStack;
using System.Threading.Tasks;

namespace $safeprojectname$.QueryServices
{
    public class TypeAheadQueryService : Service
    {
        readonly ITypeaheadSmartSearchQuery Query;

        public TypeAheadQueryService(ITypeaheadSmartSearchQuery query)
        {
            Query = query;
        }

        public async Task<object> Any(FilterTypeahead req)
        {
            return await Query.Execute(req.ConvertTo<SmartShearchQueryRequest>());
        }
    }
}