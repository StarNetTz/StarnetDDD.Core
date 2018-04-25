using System.Threading.Tasks;

namespace Starnet.Projections
{
    public interface ICheckpointReader
    {
        Task<Checkpoint> Read(string id);
    }
}
