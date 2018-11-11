using System.Threading.Tasks;

namespace $safeprojectname$
{
    public interface IQueryById<T>
    {
        Task<T> GetById(string id);
    }
}