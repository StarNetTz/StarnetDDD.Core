using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    [TestFixture]
    class BDDTests : _ServiceSpec
    {
        [Test]
        public async Task can_set_value()
        {
            string id = "persons-1";
            Given();
            When(new CreatePerson() { Id = id, Name = "Kemo" });
            await Expect(new PersonCreated() { Id = id, Name = "Kemo" });
        }

        [Test]
        public async Task can_rename_existing_person()
        {
            string id = "persons-1";
            Given(new PersonCreated() { Id = id, Name = "Kemo" });
            When(new RenamePerson() { Id = id, Name = "Munib" });
            await Expect(new PersonRenamed() { Id = id, Name = "Munib" });
        }

        [Test]
        public async Task cannot_create_person_using_id_that_is_already_assigned()
        {
            string id = "persons-1";
            Given(new PersonCreated() { Id = id, Name = "Kemo" });
            When(new CreatePerson() { Id = id, Name = "Kemo" });
            await ExpectError("AggregateAlreadyExists");
        }
    }
}
