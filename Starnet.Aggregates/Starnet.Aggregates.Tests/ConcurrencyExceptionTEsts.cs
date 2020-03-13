using NUnit.Framework;

namespace Starnet.Aggregates.Tests
{
    class ConcurrencyExceptionTests
    {
        [Test]
        public void can_instantiate_using_all_constructors()
        {
            var d = new ConcurrencyException();
            new ConcurrencyException("error message", d);
        }
    }
}