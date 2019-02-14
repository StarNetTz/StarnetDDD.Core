using NUnit.Framework;
using Starnet.Projections.Testing;
using System.Threading.Tasks;

namespace Starnet.Projections.UnitTests
{
    class InMemoryProjectionsStoreUnitTests
    {
        [Test]
        public async Task Can_Store_Document_With_Numeric_Id()
        {
            var s = new InMemoryProjectionsStore();
            var d = new DocumentWithNumericId { Id = 1 };
            await s.StoreAsync(d);
            var loaded = await s.LoadAsync<DocumentWithNumericId>("1");
            Assert.That(loaded.Id == 1);
        }

        [Test]
        public void Throws_On_Unsupporeted_Id()
        {
            var s = new InMemoryProjectionsStore();
            var d = new DocumentWithUnsupportedId { Id = 1 };
            Assert.That(async () => { await s.StoreAsync(d); }, Throws.ArgumentException);
        }
    }

    public class DocumentWithNumericId
    {
        public long Id { get; set; }
    }

    public class DocumentWithUnsupportedId
    {
        public short Id { get; set; }
    }
}
