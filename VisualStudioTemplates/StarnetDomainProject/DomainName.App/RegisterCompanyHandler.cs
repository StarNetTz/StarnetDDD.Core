using $ext_projectname$.PL.Commands;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;
using $ext_projectname$.Domain.Company;
using Starnet.Aggregates;
using System;

namespace $safeprojectname$
{
    public class RegisterCompanyHandler : IHandleMessages<RegisterCompany>
    {
        public ICompanyApplicationService Svc { get; set; }
        static ILog Log = LogManager.GetLogger<RegisterCompanyHandler>();
       
        public async Task Handle(RegisterCompany message, IMessageHandlerContext context)
        {
            try
            {
                await Svc.Execute(message);
                Log.Info($"{message.GetType()} {message.Id} {message.Name}");
            }
            catch (DomainError ex)
            {

                Log.Error(ex.Name);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}