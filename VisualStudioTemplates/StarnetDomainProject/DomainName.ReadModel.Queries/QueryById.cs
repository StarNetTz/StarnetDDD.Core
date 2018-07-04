using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class QueryById<T> : IQueryById<T>
    {
        protected readonly IDocumentStore DocumentStore;

        public QueryById(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        public async Task<T> GetById(string id)
        {
            using (var ses = DocumentStore.OpenAsyncSession())
            {
                return await ses.LoadAsync<T>(id);
            }
        }
    }
}
