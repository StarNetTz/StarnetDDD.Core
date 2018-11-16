using Raven.Client.Documents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public abstract class SmartSearchQuery<T>
    {
        protected readonly IDocumentStore DocumentStore;

        public SmartSearchQuery(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        public async Task<PaginatedResult<T>> Execute(ISmartSearchQueryRequest qry)
        {
            QueryResult<T> qResult = await SearchAsync(qry);
            var resp = CreateResponse(qry, qResult);
            if (CurrentPageIsOverflown(resp))
                return await Execute(new SmartShearchQueryRequest() { Qry = qry.Qry, CurrentPage = 0, PageSize = qry.PageSize });
            return resp;
        }

        protected abstract Task<QueryResult<T>> SearchAsync(ISmartSearchQueryRequest qry);

        protected PaginatedResult<T> CreateResponse(ISmartSearchQueryRequest request, QueryResult<T> qr)
        {
            PaginatedResult<T> retVal = new PaginatedResult<T>() { Data = new List<T>() };
            retVal.Data = qr.Data;
            retVal.TotalItems = qr.Statistics.TotalResults;
            retVal.TotalPages = retVal.TotalItems / request.PageSize;
            if ((retVal.TotalItems % request.PageSize) > 0)
                retVal.TotalPages += 1;
            retVal.PageSize = request.PageSize;
            retVal.CurrentPage = request.CurrentPage;
            return retVal;
        }

        static bool CurrentPageIsOverflown(PaginatedResult<T> result)
        {
            return (result.Data.Count == 0) && (result.TotalPages > 0);
        }
    }
}