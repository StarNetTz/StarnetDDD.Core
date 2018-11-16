using NServiceBus;
using NServiceBus.Logging;
using Starnet.Aggregates;
using System;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public abstract class AggregateHandlerBase
    {
        static ILog Log = LogManager.GetLogger<AggregateHandlerBase>();

        public async Task TryHandle(object msg, IMessageHandlerContext context, IInteractor svc)
        {
            try
            {
                await svc.Execute(msg);
                foreach (var e in svc.GetPublishedEvents())
                    await context.Publish(e).ConfigureAwait(false);
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