﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Projections
{
    public interface IProjectionsStore
    {
        Task StoreAsync(object doc);
        Task StoreInUnitOfWorkAsync(params object[] docs);
        Task<T> LoadAsync<T>(string id) where T : class;
        Task DeleteAsync(string id);
        Task StoreAsync<T>(T doc);
        Task StoreInUnitOfWorkAsync<T>(params T[] docs);
        Task<Dictionary<string, T>> LoadAsync<T>(params string[] ids) where T : class;
    }

    public interface INoSqlStore : IProjectionsStore { };
    public interface ISqlStore : IProjectionsStore { };
}