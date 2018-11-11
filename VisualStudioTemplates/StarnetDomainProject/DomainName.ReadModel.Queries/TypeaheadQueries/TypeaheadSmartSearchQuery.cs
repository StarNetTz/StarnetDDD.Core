using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace $safeprojectname$
{
    public class TypeaheadSmartSearchQuery : SmartSearchQuery<TypeaheadItem>, ITypeaheadSmartSearchQuery
    {
        public const string OrganizationsCollectionName = "organizations";
        public TypeaheadSmartSearchQuery(IDocumentStore documentStore) : base(documentStore) { }

        protected override async Task<QueryResult<TypeaheadItem>> SearchAsync(ISmartSearchQueryRequest qry)
        {
            switch (qry.Collection)
            {
                case OrganizationsCollectionName:
                    return await SearchOrganizations(qry);
                default:
                    throw new ArgumentException($"{qry.Collection} not implemented as typeahead search collection!");
            }
        }

            async Task<QueryResult<TypeaheadItem>> SearchOrganizations(ISmartSearchQueryRequest qry)
            {
                QueryResult<TypeaheadItem> retVal = new QueryResult<TypeaheadItem>();
                QueryStatistics statsRef = new QueryStatistics();
                List<TypeaheadItem> searchResult = null;
                using (var ses = DocumentStore.OpenAsyncSession())
                {
                    searchResult = await ses.Query<Organization, Organizations_Smart_Search>()
                       .Statistics(out statsRef)
                       .Search(x => x.Name, $"{qry.Qry}")
                       .Skip(qry.CurrentPage * qry.PageSize)
                       .Take(qry.PageSize)
                       .Select(x => new TypeaheadItem { Id = x.Id, Value = x.Name })
                       .ToListAsync();

                    retVal.Data = searchResult;
                    retVal.Statistics = statsRef;
                }
                return retVal;
            }
    }
}