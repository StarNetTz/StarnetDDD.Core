using NUnit.Framework;
using Starnet.Aggregates.Tests.Domain.PL.Commands;
using Starnet.Aggregates.Tests.Domain.PL.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests.PersonTests
{
    class RenamePersonTests : _ServiceSpec
    {
        [Test]
        public async Task can_rename_person()
        {
            var id = $"Persons-{Guid.NewGuid()}";
            Given(new PersonCreated() { Id = id, Name = "Kemo" });
            When(new RenamePerson() { Id = id, Name = "Munib" });
            await Expect(new PersonRenamed() { Id = id, Name = "Munib" });
        }
    }
}