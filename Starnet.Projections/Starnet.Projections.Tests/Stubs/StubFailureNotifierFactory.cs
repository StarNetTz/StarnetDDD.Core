using System.Threading.Tasks;

namespace Starnet.Projections.Tests
{
    public class StubFailureNotifierFactory : IFailureNotifierFactory
    {
        public IFailureNotifier Create()
        {
            return new StubFailureNotifier();
        }
    }

    public class StubFailureNotifier : IFailureNotifier
    {
        public Task Notify(FailureMessage message)
        {
            return Task.CompletedTask;
        }
    }
}
