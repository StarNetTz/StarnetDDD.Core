using NUnit.Framework;
using Starnet.SampleDomain;
using System;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    class RenamePersonTests : _ServiceSpec
    {
        [Test]
        public async Task can_execute_valid_command()
        {
            var id = $"Persons-{Guid.NewGuid()}";

            Given(new PersonRegistered() { Id = id, Name = "John" });
            When(new RenamePerson() { Id = id, Name = "Gary" });
            await Expect(new PersonRenamed() { Id = id, Name = "Gary" });
        }

        [Test]
        public async Task rename_is_idempotent()
        {
            var id = $"Persons-{Guid.NewGuid()}";

            Given(new PersonRegistered() { Id = id, Name = "John" });
            When(new RenamePerson() { Id = id, Name = "John" });
            await Expect();
        }
    }
}