using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    class BDDTests : _ServiceSpec
    {
        [Test]
        public async Task can_create()
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
        public async Task cannot_re_register()
        {
            var id = $"Persons-{Guid.NewGuid()}";

            Given(new PersonRegistered() { Id = id, Name = "John" });
            When(new RegisterPerson() { Id = id, Name = "John" });
            await ExpectError("PersonAlreadyRegistered");
        }

        [Test]
        public async Task can_rename()
        {
            var id = $"Persons-{Guid.NewGuid()}";

            Given(new PersonRegistered() { Id = id, Name = "John" });
            When(new RenamePerson() { Id = id, Name = "Munib" });
            await Expect(new PersonRenamed() { Id = id, Name = "Munib" });
        }
    }
}