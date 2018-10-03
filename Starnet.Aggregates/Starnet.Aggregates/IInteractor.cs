using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Aggregates
{
    public interface IInteractor
    {
        Task Execute(object command);
        List<object> GetPublishedEvents();
    }
}