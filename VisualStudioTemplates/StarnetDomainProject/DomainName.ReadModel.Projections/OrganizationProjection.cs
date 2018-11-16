using $ext_projectname$.PL.Events;
using Starnet.Projections;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    [SubscribesToStream("$ce-Organizations")]
    public class OrganizationProjection : Projection, IHandledBy<OrganizationProjectionHandler> { }

    public class OrganizationProjectionHandler : IHandler
    {
        readonly INoSqlStore Store;

        public OrganizationProjectionHandler(INoSqlStore store)
        {
            Store = store;
        }

        public async Task Handle(dynamic @event, long checkpoint)
        {
            await When(@event, checkpoint);
        }

        public async Task When(OrganizationRegistered e, long checkpoint)
        {
            Organization doc = await Store.LoadAsync<Organization>(e.Id);
            if (doc == null)
                doc = new Organization();
            doc.Id = e.Id;
            doc.Name = e.Name;
            doc.Address = e.Address;
            await Store.StoreAsync(doc);
        }
    }
}
