using NUnit.Framework;
using Starnet.Aggregates.Tests.Domain.PL.Commands;
using Starnet.Aggregates.Tests.Domain.PL.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests.PersonTests
{
    class CreatePersonTests : _ServiceSpec
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
        public async Task cannot_create_person_using_preexisting_id()
        {
            var id = $"Persons-{Guid.NewGuid()}";
            Given(new PersonCreated() { Id = id, Name = "Kemo" });
            When(new CreatePerson() { Id = id, Name = "Kemo" });
            await ExpectError("AggregateAlreadyExists");
        }
    }
}