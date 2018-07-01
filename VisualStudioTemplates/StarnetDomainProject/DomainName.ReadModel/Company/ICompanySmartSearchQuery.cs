using System.Threading.Tasks;

namespace $safeprojectname$
{
    public interface ICompanySmartSearchQuery
    {
        Task<PaginatedResult<Company>> Execute(ISmartSearchQueryRequest qry);
    }
}
