using NUnit.Framework;
using Starnet.Projections.Testing;
using Starnet.Projections.Tests;
using System;
using System.Threading.Tasks;

namespace Starnet.Projections.UnitTests
{
    [TestFixture]
    public class CompanyTests : ProjectionSpecification<TestProjection, TestModel>
    {
        string Id;

        [SetUp]
        public void SetUp()
        {
            Id = $"Company-{Guid.NewGuid()}";
        }

        [Test]
        public async Task can_project_event()
        {
            await Given(new TestEvent() { Id = Id, SomeValue = "123" });
            await Expect(new TestModel() { Id = Id, SomeValue = "123" });
        }
    }
}