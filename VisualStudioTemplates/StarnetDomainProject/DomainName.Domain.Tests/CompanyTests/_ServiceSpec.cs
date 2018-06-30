using $ext_projectname$.Domain.Company;
using $ext_projectname$.PL.Commands;
using $ext_projectname$.PL.Events;
using Starnet.Aggregates.Testing;
using System.Linq;
using System.Threading.Tasks;

namespace $safeprojectname$.CompanyTests
{
    internal class _ServiceSpec : ApplicationServiceSpecification<ICommand, IEvent>
    {
        protected override async Task<IEvent[]> ExecuteCommand(IEvent[] given, ICommand cmd)
        {
            var repository = new BDDAggregateRepository();
            repository.Preload(cmd.Id, given);
            var svc = new CompanyApplicationService(repository);
            await svc.Execute(cmd).ConfigureAwait(false);
            var arr = repository.Appended != null ? repository.Appended.Cast<IEvent>().ToArray() : null;
            return arr ?? new IEvent[0];
        }
    }
}