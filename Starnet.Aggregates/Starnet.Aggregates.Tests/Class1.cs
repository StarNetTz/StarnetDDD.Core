using NUnit.Framework;

namespace Starnet.Aggregates.Tests
{
    class DomainErrorTests
    {
        [Test]
        public void CanInstantiate()
        {
            var d = new DomainError();
        }
    }
}
