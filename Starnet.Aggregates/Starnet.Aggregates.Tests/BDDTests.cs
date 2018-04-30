using NUnit.Framework;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    [TestFixture]
    class BDDTests : _ServiceSpec
    {
        const string AggregateId = "persons-1";

        [Test]
        public async Task can_create_person()
        {
            Given();
            When(new CreatePerson() { Id = AggregateId, Name = "Kemo" });
            await Expect(new PersonCreated() { Id = AggregateId, Name = "Kemo" });
        }

        [Test]
        public async Task can_rename_person()
        {
            Given(new PersonCreated() { Id = AggregateId, Name = "Kemo" });
            When(new RenamePerson() { Id = AggregateId, Name = "Munib" });
            await Expect(new PersonRenamed() { Id = AggregateId, Name = "Munib" });
        }

        [Test]
        public async Task cannot_create_person_using_preexisting_id()
        {
            Given(new PersonCreated() { Id = AggregateId, Name = "Kemo" });
            When(new CreatePerson() { Id = AggregateId, Name = "Kemo" });
            await ExpectError("AggregateAlreadyExists");
        }
    }
}