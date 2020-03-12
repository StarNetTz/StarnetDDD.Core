using NUnit.Framework;

namespace Starnet.Aggregates.Tests
{
    class ConcurrencyExceptionTests
    {
        [Test]
        public void Can_instantiate()
        {
            var d = new ConcurrencyException();
            new ConcurrencyException("error message", d);
        }
    }
}