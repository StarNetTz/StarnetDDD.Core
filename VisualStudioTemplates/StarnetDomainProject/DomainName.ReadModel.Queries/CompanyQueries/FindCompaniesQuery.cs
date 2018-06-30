using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class FindCompaniesQuery : IFindCompaniesQuery
    {
        readonly IDocumentStore DocumentStore;

        public FindCompaniesQuery(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        public async Task<PaginatedResult<Company>> Execute(ISmartSearchQueryRequest qry)
        {
            QueryResult<Company> qResult = await Search(qry);
            var resp = CreateResponse(qry, qResult);
            if (CurrentPageIsOverflown(resp))
                return await Execute(new SmartShearchQueryRequest() { Qry = qry.Qry, CurrentPage = 0, PageSize = qry.PageSize });
            return resp;
        }

            async Task<QueryResult<Company>> Search(ISmartSearchQueryRequest qry)
            {
                QueryResult<Company> retVal = new QueryResult<Company>();
                QueryStatistics statsRef = new QueryStatistics();
                using (var ses = DocumentStore.OpenAsyncSession())
                {
                    var searchResult = await ses.Query<Company, Companies_Smart_Search>()
                       .Statistics(out statsRef)
                       .Search(x => x.Name, $"{qry.Qry}")
                       .Skip(qry.CurrentPage * qry.PageSize)
                       .Take(qry.PageSize)
                       .ToListAsync();

                    retVal.Data = searchResult;
                    retVal.Statistics = statsRef;
                }
                return retVal;
            }

            PaginatedResult<Company> CreateResponse(ISmartSearchQueryRequest request, QueryResult<Company> qr)
            {
                PaginatedResult<Company> retVal = new PaginatedResult<Company>() { Data = new List<Company>() };
                retVal.Data = qr.Data;
                retVal.TotalItems = qr.Statistics.TotalResults;
                retVal.TotalPages = retVal.TotalItems / request.PageSize;
                if ((retVal.TotalItems % request.PageSize) > 0)
                    retVal.TotalPages += 1;
                retVal.PageSize = request.PageSize;
                retVal.CurrentPage = request.CurrentPage;
                return retVal;
            }

            static bool CurrentPageIsOverflown(PaginatedResult<Company> result)
            {
                return (result.Data.Count == 0) && (result.TotalPages > 0);
            }
    }

   
    public class Companies_Smart_Search : AbstractMultiMapIndexCreationTask<Company>
    {
        public Companies_Smart_Search()
        {
            AddMap<Company>(companies => from c in companies
                                           select new
                                           {
                                               c.Id,
                                               c.Name
                                           });

            Index(x => x.Name, FieldIndexing.Search);
        }
    }
}