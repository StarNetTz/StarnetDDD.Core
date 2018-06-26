using System.Threading.Tasks;

namespace Starnet.Projections.Tests
{
    public class TestHandler : IHandler
    {
        readonly INoSqlStore Store;

        public TestHandler(INoSqlStore store)
        {
            Store = store;
        }

        public async Task Handle(dynamic @event, long checkpoint)
        {
            await When(@event, checkpoint);
        }

        public async Task When(TestEvent e, long checkpoint)
        {
            var doc = new TestModel { Id = e.Id, SomeValue = e.SomeValue };
            await Store.StoreAsync(doc);
        }
    }
}