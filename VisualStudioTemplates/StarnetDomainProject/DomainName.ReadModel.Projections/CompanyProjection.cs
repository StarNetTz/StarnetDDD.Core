using $ext_projectname$.PL.Events;
using Starnet.Projections;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    [SubscribesToStream("$ce-Companies")]
    public class CompanyProjection : Projection, IHandledBy<CompanyProjectionHandler> { }

    public class CompanyProjectionHandler : IHandler
    {
        readonly INoSqlStore Store;

        public CompanyProjectionHandler(INoSqlStore store)
        {
            Store = store;
        }

        public async Task Handle(dynamic @event, long checkpoint)
        {
            await When(@event, checkpoint);
        }

        public async Task When(CompanyRegistered e, long checkpoint)
        {
            Company doc = await Store.LoadAsync<Company>(e.Id);
            if (doc == null)
                doc = new Company();
            doc.Id = e.Id;
            doc.Name = e.Name;
            doc.Address = e.Address;
            doc.VATId = e.VATId;
            await Store.StoreAsync(doc);
        }
    }
}
