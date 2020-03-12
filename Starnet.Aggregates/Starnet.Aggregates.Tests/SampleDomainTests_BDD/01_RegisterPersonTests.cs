using NUnit.Framework;
using Starnet.SampleDomain;
using System;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    class RegisterPersonTests : _ServiceSpec
    {
        [Test]
        public async Task can_execute_valid_command()
        {
            var id = $"Persons-{Guid.NewGuid()}";
            var ev = new PersonRegistered() { Id = id, Name = "John" };
            var expectedProducedEvents = ToEventList(ev);
            var expectedPublishedEvents = expectedProducedEvents;

            Given();
            When(new RegisterPerson() { Id = id, Name = "John" });
            await Expect(expectedProducedEvents, expectedPublishedEvents);
        }

        [Test]
        public async Task registration_is_idempotent()
        {
            var id = $"Persons-{Guid.NewGuid()}";

            Given(new PersonRegistered() { Id = id, Name = "John" });
            When(new RegisterPerson() { Id = id, Name = "John" });
            await Expect();
        }

        [Test]
        public async Task non_idempotent_registration_throws()
        {
            var id = $"Persons-{Guid.NewGuid()}";

            Given(new PersonRegistered() { Id = id, Name = "John" });
            When(new RegisterPerson() { Id = id, Name = "Danny" });
            await ExpectError("PersonAlreadyRegistered");
        }
    }
}