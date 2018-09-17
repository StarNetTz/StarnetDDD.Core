using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Starnet.Aggregates.InMemoryAggregateRepository;

namespace Starnet.Aggregates.Tests
{
    [TestFixture]
    class InMemoryRepositoryAsycTests
    {
        InMemoryAggregateRepository Repository;

        [SetUp]
        public void Setup()
        {
            Repository = new InMemoryAggregateRepository();
        }

        [Test]
        public async Task loading_with_invalid_id_returns_null()
        {
            var agg = await Repository.GetAsync<PersonAggregate>("");
            Assert.That(agg, Is.Null);
        }

        [Test]
        public async Task can_store_and_load_aggregate()
        {
            string id = "1";
            await Repository.StoreAsync(CreatePersonAggregate(id, "Mujo"));
            var pl = await Repository.GetAsync<PersonAggregate>(id);

            Assert.That(pl.Version, Is.EqualTo(1));
        }

        [Test]
        public async Task saving_an_aggregate_clears_list_of_changes()
        {
            var p = CreatePersonAggregate("1", "Mujo");
            await Repository.StoreAsync(p);
            Assert.That(p.Changes, Is.Empty);
        }



        [Test]
        public async Task can_load_requested_version_of_aggregate()
        {
            string id = "156";
            int nrOfUpdates = 5;
            await CreateUpdatedAggregate(id, nrOfUpdates);
            var p = await Repository.GetAsync<PersonAggregate>(id, 3);
            Assert.That(p.Version, Is.EqualTo(3));
        }

        async Task CreateUpdatedAggregate(string aggId, int nrOfUpdates)
        {
            var p = new PersonAggregate(new PersonAggregateState());
            p.Create(new CreatePerson() { Id = aggId, Name = "0" }, new List<object>());
            for (int i = 0; i < nrOfUpdates; i++)
                p.Rename(new RenamePerson() { Id = aggId, Name = $"Name {i + 1}" }, new List<object>());
            await Repository.StoreAsync(p);
        }


        [Test]
        public async Task concurrent_updates_cause_AggregateConcurrencyException()
        {
            var id = "21";
            await Repository.StoreAsync(CreatePersonAggregate(id, "Mujo"));
            var loadedAggregate = await Repository.GetAsync<PersonAggregate>(id);
            await OutOfSessionUpdate(id);

            InSessionUpdate(loadedAggregate);

            Assert.That(async () => await Repository.StoreAsync(loadedAggregate), Throws.Exception.TypeOf<AggregateConcurrencyException>());
        }

        async Task OutOfSessionUpdate(string id)
        {
            var agg = await Repository.GetAsync<PersonAggregate>(id);
            agg.Rename(new RenamePerson() { Id = id, Name = "Renamed out of session" }, new List<object>());
            await Repository.StoreAsync(agg);
        }

        void InSessionUpdate(PersonAggregate agg)
        {
            var cmd = new RenamePerson() { Id = agg.Id, Name = "Renamed in session" };
            agg.Rename(cmd, new List<object>());
        }

        PersonAggregate CreatePersonAggregate(string id, string name)
        {
            var agg = new PersonAggregate(new PersonAggregateState());
            agg.Create(new CreatePerson() { Id = id, Name = name }, new List<object>());
            return agg;
        }
    }
}