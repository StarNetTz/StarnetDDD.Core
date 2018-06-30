using System.Threading.Tasks;

namespace $safeprojectname$
{
    public interface IFindCompaniesQuery
    {
        Task<PaginatedResult<Company>> Execute(ISmartSearchQueryRequest qry);
    }
}
