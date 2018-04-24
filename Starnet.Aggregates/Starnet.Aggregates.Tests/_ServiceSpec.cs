using Starnet.Aggregates.Testing;
using System.Linq;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    internal class _ServiceSpec : ApplicationServiceSpecification<ICommand, IEvent>
    {
        protected override async Task<IEvent[]> ExecuteCommand(IEvent[] given, ICommand cmd)
        {
            var repository = new BDDAggregateRepository();
            repository.Preload(cmd.Id, given);
            var svc = new PersonAggregateApplicationService(repository);
            await svc.Execute(cmd);
            var arr = (repository.Appended != null) ? repository.Appended.Cast<IEvent>().ToArray() : null;
            return arr ?? new IEvent[0];
        }
    }
}
