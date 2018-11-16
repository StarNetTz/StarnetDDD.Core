using System.Threading.Tasks;

namespace $safeprojectname$
{
    public interface IOrganizationSmartSearchQuery
    {
        Task<PaginatedResult<Organization>> Execute(ISmartSearchQueryRequest qry);
    }
}
