using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Starnet.Aggregates.GetEventStore.Tests
{
    [TestFixture]
    class GESAggregateRepositoryTests
    {
        private IEventStoreConnection Connection;
        private GESAggregateRepository Repository;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await InitializeConnection();
            Repository = new GESAggregateRepository(Connection);
        }

        [Test]
        public async Task can_store_aggregate()
        {
            var id = $"persons-{Guid.NewGuid()}";
            var agg = CreatePersonAggregate(id, "Zvjezdan");
            await Repository.StoreAsync(agg);

        }

        [Test]
        public async Task can_store_aggregate_using_pagination_with_over_500_events()
        {
            var id = $"persons-{Guid.NewGuid()}";
            var agg = CreatePersonAggregate(id, "Zvjezdan");
            for (int i = 0; i < 510; i++)
                agg.Rename(new RenamePerson() { Id = id, Name = $"Name {i + 1}" });
            await Repository.StoreAsync(agg);
        }

        [Test]
        public async Task storing_an_aggregate_resets_list_of_changes()
        {
            var id = $"persons-{Guid.NewGuid()}";
            var agg = CreatePersonAggregate(id, "Zvjezdan");
            agg.Rename(new RenamePerson() { Id = id, Name = "Zeka peka" });

            await Repository.StoreAsync(agg);

            Assert.That(agg.Changes, Is.Empty);
        }

        [Test]
        public async Task can_load_stored_aggregate()
        {
            var id = $"persons-{Guid.NewGuid()}";
            var agg = CreatePersonAggregate(id, "Zvjezdan");
            agg.Rename(new RenamePerson() { Id = id, Name = "Zeka peka" });
            await Repository.StoreAsync(agg);

            var loadedAggregate = await Repository.GetAsync<PersonAggregate>(agg.Id);

            Assert.That(loadedAggregate.Version, Is.EqualTo(2));
        }

        [Test]
        public async Task can_load_aggregate_using_pagination_with_over_500_events()
        {
            var id = $"persons-{Guid.NewGuid()}";
            var agg = CreatePersonAggregate(id, "Zvjezdan");
            for (int i = 0; i < 509; i++)
                agg.Rename(new RenamePerson() { Id = id, Name = $"Name {i + 1}" });
            await Repository.StoreAsync(agg);
            var loadedAggregate = await Repository.GetAsync<PersonAggregate>(id);
            Assert.That(loadedAggregate.Version, Is.EqualTo(510));
        }

        [Test]
        public async Task can_load_requested_version_of_aggregate()
        {
            var id = $"persons-{Guid.NewGuid()}";
            var agg = CreatePersonAggregate(id, "Zvjezdan");
            for (int i = 0; i < 5; i++)
                agg.Rename(new RenamePerson() { Id = id, Name = $"Name {i + 1}" });

            await Repository.StoreAsync(agg);

            var loadedAggregate = await Repository.GetAsync<PersonAggregate>(id, 2);

            Assert.That(agg.Version, Is.EqualTo(6));
            Assert.That(loadedAggregate.Version, Is.EqualTo(2));
        }

        [Test]
        public async Task cannot_load_aggregate_with_invalid_version_param()
        {
            var id = $"persons-{Guid.NewGuid()}";
            var agg = CreatePersonAggregate(id, "Zvjezdan");
            agg.Rename(new RenamePerson() { Id = id, Name = "Zeka peka" });
            await Repository.StoreAsync(agg);
            Assert.That(async () => await Repository.GetAsync<PersonAggregate>(agg.Id, -1), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task loading_with_invalid_id_returns_null()
        {
            var id = $"persons-{Guid.NewGuid()}";
            var agg = await Repository.GetAsync<PersonAggregate>(id);
            Assert.That(agg, Is.Null);
        }

        [Test]
        public async Task concurrent_updates_cause_WrongExpectedVersionException()
        {
            var id = $"persons-{Guid.NewGuid()}";
            await InitializeAggregate(id);
            var loadedAggregate = await Repository.GetAsync<PersonAggregate>(id);
            await UpdateAggOutOfThisTransaction(id);
            UpdateAggInThisTransaction(loadedAggregate);

            try
            {
                await Repository.StoreAsync(loadedAggregate); Assert.Fail();
            } //dirty hack, normal throw async fails to finish test
            catch (Exception ex)
            {

                Assert.That(ex is WrongExpectedVersionException);
            }
        }

        private async Task InitializeConnection()
        {
            Connection = EventStoreConnection.Create(EventStoreConnectionSettings.TcpEndpoint);
            await Connection.ConnectAsync();
        }

        private PersonAggregate CreatePersonAggregate(string id, string name)
        {
            var p = new PersonAggregate(new PersonAggregateState());
            p.Create(new CreatePerson() { Id = id, Name = name });
            return p;
        }

        private async Task InitializeAggregate(string id)
        {

            await Repository.StoreAsync(CreatePersonAggregate(id, "Zeko"));
        }

        private void UpdateAggInThisTransaction(PersonAggregate agg)
        {
            var cmd = new RenamePerson() { Id = agg.Id, Name = "new value" };
            agg.Rename(cmd);
        }

        private async Task UpdateAggOutOfThisTransaction(string id)
        {
            var agg = await Repository.GetAsync<PersonAggregate>(id);
            agg.Rename(new RenamePerson() { Id = id, Name = "new value" });
            await Repository.StoreAsync(agg);
        }
    }
}
