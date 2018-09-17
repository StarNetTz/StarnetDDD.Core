using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Aggregates
{
    public interface IApplicationService
    {
        Task Execute(object command);
        List<object> PublishedEvents { get; }
    }
}