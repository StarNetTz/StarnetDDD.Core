using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System.Linq;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class OrganizationSmartSearchQuery : SmartSearchQuery<Organization>, IOrganizationSmartSearchQuery
    {
        public OrganizationSmartSearchQuery(IDocumentStore documentStore) : base(documentStore) { }

        protected override async Task<QueryResult<Organization>> Search(ISmartSearchQueryRequest qry)
        {
            QueryResult<Organization> retVal = new QueryResult<Organization>();
            QueryStatistics statsRef = new QueryStatistics();
            using (var ses = DocumentStore.OpenAsyncSession())
            {
                var searchResult = await ses.Query<Organization, Organizations_Smart_Search>()
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

    public class Organizations_Smart_Search : AbstractMultiMapIndexCreationTask<Organization>
    {
        public Organizations_Smart_Search()
        {
            AddMap<Organization>(companies => from c in companies
                                         select new
                                         {
                                             c.Id,
                                             c.Name
                                         });

            Index(x => x.Name, FieldIndexing.Search);
        }
    }
}