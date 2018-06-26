using System.Threading.Tasks;

namespace Starnet.Projections
{
    public interface IProjectionsStore
    {
        Task StoreAsync(object doc);
        Task<T> LoadAsync<T>(string id) where T : class;
        Task DeleteAsync(string id);
        Task StoreAsync<T>(T doc);
    }

    public interface INoSqlStore : IProjectionsStore { };
    public interface ISqlStore : IProjectionsStore { };
}