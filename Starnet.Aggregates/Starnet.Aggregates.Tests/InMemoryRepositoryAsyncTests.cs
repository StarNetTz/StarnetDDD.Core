using NUnit.Framework;
using Starnet.SampleDomain;
using System;
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
            var id = $"Persons-{Guid.NewGuid()}";
            await Repository.StoreAsync(PersonAggregateFactory.Create(id, "Joe"));
            var pl = await Repository.GetAsync<PersonAggregate>(id);
            Assert.That(pl.Version, Is.EqualTo(1));
        }

        [Test]
        public async Task saving_an_aggregate_clears_list_of_changes()
        {
            var id = $"Persons-{Guid.NewGuid()}";
            var p = PersonAggregateFactory.Create(id, "Mary");
            await Repository.StoreAsync(p);
            Assert.That(p.Changes, Is.Empty);
        }

        [Test]
        public async Task can_load_requested_version_of_aggregate()
        {
            var id = $"Persons-{Guid.NewGuid()}";
            int finalVersion = 5;
            int targetVersion = 3;
            await CreateAndStoreUpdatedAggregate(id, finalVersion);
            var agg = await Repository.GetAsync<PersonAggregate>(id, targetVersion);
            Assert.That(agg.Version, Is.EqualTo(3));
        }

            async Task CreateAndStoreUpdatedAggregate(string aggId, int nrOfUpdates)
            {
                var p = PersonAggregateFactory.CreateWithUncommitedUpdates(aggId, nrOfUpdates);
                await Repository.StoreAsync(p);
            }

        [Test]
        public async Task concurrent_updates_throw_AggregateConcurrencyException()
        {
           var id = $"Persons-{Guid.NewGuid()}";
           await Repository.StoreAsync(PersonAggregateFactory.Create(id, "Joe"));
           var loadedAggregate = await Repository.GetAsync<PersonAggregate>(id);
           await TestingUtils.UpdateOutOfSession(id, Repository);
           TestingUtils.Rename(loadedAggregate, "Gary");

           Assert.That(async () => await Repository.StoreAsync(loadedAggregate), Throws.Exception.TypeOf<AggregateConcurrencyException>());
        }
    }
}