using System.Threading.Tasks;

namespace Starnet.Projections
{
    public interface IHandler
    {
        Task Handle(dynamic @event, long checkpoint);
    }
}
