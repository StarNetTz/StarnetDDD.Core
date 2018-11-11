using $ext_projectname$.Domain.Organization;
using $ext_projectname$.PL.Commands;
using NServiceBus;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class RegisterOrganizationHandler : AggregateHandlerBase, IHandleMessages<RegisterOrganization>
    {
        readonly IOrganizationInteractor Svc;

        public RegisterOrganizationHandler(IOrganizationInteractor svc)
        {
            Svc = svc;
        }

        public async Task Handle(RegisterOrganization message, IMessageHandlerContext context)
        {
            await TryHandle(message, context, Svc);
        }
    }
}