using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System.Linq;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class CompanySmartSearchQuery : SmartSearchQuery<Company>, ICompanySmartSearchQuery
    {
        public CompanySmartSearchQuery(IDocumentStore documentStore):base(documentStore)  {}

        protected override async Task<QueryResult<Company>> Search(ISmartSearchQueryRequest qry)
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