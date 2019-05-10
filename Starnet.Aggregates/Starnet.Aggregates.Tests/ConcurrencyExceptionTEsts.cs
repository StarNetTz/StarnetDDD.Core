using NUnit.Framework;

namespace Starnet.Aggregates.Tests
{
    class ConcurrencyExceptionTests
    {
        [Test]
        public void CanInstantiate()
        {
            var d = new ConcurrencyException();
            var d2 = new ConcurrencyException("error", d);
        }
    }
}
