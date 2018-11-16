using System.Threading.Tasks;

namespace $safeprojectname$
{
    public interface ITypeaheadSmartSearchQuery
    {
        Task<PaginatedResult<TypeaheadItem>> Execute(ISmartSearchQueryRequest qry);
    }
}
