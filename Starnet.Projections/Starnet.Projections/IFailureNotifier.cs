using System.Threading.Tasks;

namespace Starnet.Projections
{
    public interface IFailureNotifier
    {
        Task Notify(FailureMessage message);
    }

    public class FailureMessage
    {
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
