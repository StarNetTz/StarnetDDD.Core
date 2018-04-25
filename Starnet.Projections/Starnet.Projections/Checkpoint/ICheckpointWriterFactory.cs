using System.Threading.Tasks;

namespace Starnet.Projections
{
    public interface ICheckpointWriterFactory
    {
        Task<ICheckpointWriter> Create();
    }
}
