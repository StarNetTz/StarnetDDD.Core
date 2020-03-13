using NUnit.Framework;

namespace Starnet.Aggregates.Tests
{
    class DomainErrorTests
    {
        [Test]
        public void can_create_using_default_constructor()
        {
            var de = new DomainError();
        }
    }
}