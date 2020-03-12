using Starnet.Aggregates.Testing;
using Starnet.SampleDomain;
using System.Linq;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    internal class _ServiceSpec : ApplicationServiceSpecification<ICommand, IEvent>
    {
        protected override async Task<ExecuteCommandResult<IEvent>> ExecuteCommand(IEvent[] given, ICommand cmd)
        {
            var repository = new BDDAggregateRepository();
            repository.Preload(cmd.Id, given);
            var svc = new PersonAggregateInteractor(repository);
            await svc.Execute(cmd);
            var publishedEvents = svc.GetPublishedEvents();
            var arr = (repository.Appended != null) ? repository.Appended.Cast<IEvent>().ToArray() : null;
            var res = new ExecuteCommandResult<IEvent> { ProducedEvents = arr ?? new IEvent[0], PublishedEvents = publishedEvents.Cast<IEvent>().ToArray() };
            return res;
        }
    }
}
