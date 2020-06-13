using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SimpleInjector;
using Starnet.Projections.Testing;
using Starnet.Projections.Tests;
using System;
using System.Threading.Tasks;

namespace Starnet.Projections.UnitTests
{
    [TestFixture]
    public class ProjectionSpecificationTests2 : ProjectionSpecification<TestProjection>
    {
        string Id;

        protected override void ConfigureContainer(IServiceCollection services)
        {
            base.ConfigureContainer(services);
            services.AddTransient<ITimeProvider, MockTimeProvider>();
            services.AddTransient<FailingHandler>();
            services.AddTransient<TestHandler>();
        }

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

        [Test]
        public void can_project_event1()
        {
            Assert.That(async () =>
            {
                await Given(new TestEvent() { Id = Id, SomeValue = "123" });
                await Expect(new TestModelWithUnsupportedIdType() { Id = 1, SomeValue = "123" }); }, Throws.ArgumentException);
        }


        [Test]
        public void unexpected_result_throws_assertion_exception()
        {
            Assert.That(ExecuteFailingTest(), Throws.InstanceOf<AssertionException>());
        }

        NUnit.Framework.Constraints.ActualValueDelegate<Task> ExecuteFailingTest()
        {
            return async () =>
            {
                await Given(new TestEvent() { Id = Id, SomeValue = "123" });
                await Expect(new TestModel() { Id = Id, SomeValue = "1234" });};
        }
    }
}