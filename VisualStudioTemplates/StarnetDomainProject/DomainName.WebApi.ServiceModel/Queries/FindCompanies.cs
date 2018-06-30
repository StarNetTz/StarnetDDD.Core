using $ext_projectname$.ReadModel;
using ServiceStack;

namespace $safeprojectname$
{
    [Route("/companies", Verbs = "GET")]
    public class FindCompanies : IReturn<PaginatedResult<Company>>
    {
        public string Qry { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
