using System.Threading.Tasks;

namespace Starnet.Projections
{
    public interface ICheckpointWriter
    {
        Task Write(Checkpoint checkpoint);
    }
}
