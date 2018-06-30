using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace $safeprojectname$
{
    class QueryResult<T>
    {
        public List<T> Data { get; set; }

        public QueryStatistics Statistics { get; set; }

        public QueryResult()
        {
            Data = new List<T>();
        }
    }
}