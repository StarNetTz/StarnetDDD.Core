using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    class BDDTests : _ServiceSpec
    {
        [Test]
        public async Task can_create_person()
        {
            var id = $"Persons-{Guid.NewGuid()}";
            Given();
            When(new CreatePerson() { Id = id, Name = "Kemo" });
            var ev = new PersonCreated() { Id = id, Name = "Kemo" };
            var expectedProducedEvents = ToEventList(ev);
            var expectedPublishedEvents = expectedProducedEvents;
            await Expect(expectedProducedEvents, expectedPublishedEvents);
        }

        [Test]
        public async Task can_rename_person()
        {
            var id = $"Persons-{Guid.NewGuid()}";
            Given(new PersonCreated() { Id = id, Name = "Kemo" });
            When(new RenamePerson() { Id = id, Name = "Munib" });
            await Expect(new PersonRenamed() { Id = id, Name = "Munib" });
        }

        [Test]
        public async Task cannot_create_person_using_preexisting_id()
        {
            var id = $"Persons-{Guid.NewGuid()}";
            Given(new PersonCreated() { Id = id, Name = "Kemo" });
            When(new CreatePerson() { Id = id, Name = "Kemo" });
            await ExpectError("AggregateAlreadyExists");
        }
    }
}