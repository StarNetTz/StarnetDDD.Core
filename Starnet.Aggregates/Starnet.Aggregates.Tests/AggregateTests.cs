using NUnit.Framework;
using Starnet.Aggregates.Tests.Domain.PL.Commands;
using Starnet.Aggregates.Tests.Domain.PL.Events;

namespace Starnet.Aggregates.Tests
{
    class AggregateTests
    {
        PersonAggregate Person;

        [SetUp]
        public void Setup()
        {
            Person = new PersonAggregate(new PersonAggregateState());
        }

        [Test]
        public void new_aggregate_is_properly_initialized()
        {
            Assert.That(Person.Id, Is.Null);
            Assert.That(Person.Version, Is.Zero);
            Assert.That(Person.Changes, Is.Empty);
        }

        [Test]
        public void can_create_person()
        {
            Person.Create(new CreatePerson() { Id = "1", Name = "Zvjezdan" });
            AssertPersonCreated(Person);
            Assert.That(Person.PublishedEvents.Count, Is.EqualTo(1));
        }

            static void AssertPersonCreated(PersonAggregate person)
            {
                Assert.That(person.Id, Is.EqualTo("1"));
                Assert.That(person.Version, Is.EqualTo(1));
                var e = person.Changes[0] as PersonCreated;
                Assert.That(e.Id, Is.EqualTo("1"));
                Assert.That(e.Name, Is.EqualTo("Zvjezdan"));
            }

        [Test]
        public void can_rename_person()
        {
            Person.Create(new CreatePerson() { Id = "1", Name = "Zvjezdan" });

            Person.Rename(new RenamePerson() { Id = "1", Name = "Muriz" });

            AssertPersonRenamed(Person);
        }

            static void AssertPersonRenamed(PersonAggregate person)
            {
                Assert.That(person.Id, Is.EqualTo("1"));
                Assert.That(person.Version, Is.EqualTo(2));
                var e = person.Changes[1] as PersonRenamed;
                Assert.That(e.Name, Is.EqualTo("Muriz"));
            }

        [Test]
        public void rename_with_invalid_name_throws_domain_error()
        {
            Person.Create(new CreatePerson() { Id = "1", Name = "Zvjezdan" });
            var exception = Assert.Throws<DomainError>(() => { Person.Rename(new RenamePerson() { Id = "1", Name = "" }); });
            Assert.That(exception.Name, Is.EqualTo("InvalidPersonName"));
        }

        [Test]
        public void aggregate_state_factory_given_aggregate_creates_state()
        {
            var state = AggregateStateFactory.CreateStateFor(typeof(PersonAggregate));
            Assert.That(state is PersonAggregateState);
        }
    }
}