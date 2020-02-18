using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Starnet.Aggregates.ES.Tests
{
    [TestFixture]
    class ESAggregateRepositoryTests
    {
        IEventStoreConnection Connection;
        ESAggregateRepository Repository;

        const int EventStoreTcpPort = 1113;
        const string EventStoreIp = "127.0.0.1";

        const int WritePageSize = 500;
        const int OverflownPageSize = WritePageSize + 20;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await InitializeConnection();
            Repository = new ESAggregateRepository(Connection);
        }

            async Task InitializeConnection()
            {
                Connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Parse(EventStoreIp), EventStoreTcpPort));
                await Connection.ConnectAsync();
            }

        [Test]
        public async Task can_store_and_load_aggregates_below_page_size_treshold()
        {
            var id = $"persons-{Guid.NewGuid()}";
            await CreateAndStoreAggregate(id);

            var agg = await Repository.GetAsync<PersonAggregate>(id);
            Assert.That(agg.Version, Is.EqualTo(1));
        }

        [Test]
        public async Task can_store_and_load_aggregates_above_page_size_treshold()
        {
            var id = $"persons-{Guid.NewGuid()}";
            await CreateAndPersistAggregateThatExceedesPageSizeTreshold(id);
            var loadedAggregate = await Repository.GetAsync<PersonAggregate>(id);
            const int ExpectedVersion = OverflownPageSize + 1;
            Assert.That(loadedAggregate.Version, Is.EqualTo(ExpectedVersion));
        }

            async Task CreateAndPersistAggregateThatExceedesPageSizeTreshold(string id)
            {
                var agg = CreatePersonAggregate(id, "Zvjezdan", OverflownPageSize);
                await Repository.StoreAsync(agg);
            }

        [Test]
        public async Task storing_an_aggregate_resets_list_of_changes()
        {
            var agg = CreatePersonAggregate($"persons-{Guid.NewGuid()}", "Zvjezdan");
            await Repository.StoreAsync(agg);
            Assert.That(agg.Changes, Is.Empty);
        }

        [Test]
        public async Task can_load_requested_version_of_aggregate()
        {
            const int NrOfEventsToAdd = 5;
            const int FinalVersion = 6;
            const int RequestedVersion = 2;

            var id = $"persons-{Guid.NewGuid()}";

            var agg = CreatePersonAggregate(id, "Zvjezdan", NrOfEventsToAdd);
            await Repository.StoreAsync(agg);

            var loadedAggregate = await Repository.GetAsync<PersonAggregate>(id, RequestedVersion);

            Assert.That(agg.Version, Is.EqualTo(FinalVersion));
            Assert.That(loadedAggregate.Version, Is.EqualTo(RequestedVersion));
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
            var agg = await Repository.GetAsync<PersonAggregate>(id);
            await UpdateAggregateOutOfTransaction(id);
            UpdateAggregate(agg);
            Assert.That(async () => { await Repository.StoreAsync(agg); }, Throws.Exception.TypeOf<ConcurrencyException>());
        }

            async Task InitializeAggregate(string id)
            {
                await Repository.StoreAsync(CreatePersonAggregate(id, "Zeko"));
            }

            async Task UpdateAggregateOutOfTransaction(string id)
            {
                var agg = await Repository.GetAsync<PersonAggregate>(id);
                agg.Rename(new RenamePerson() { Id = id, Name = "new value" });
                await Repository.StoreAsync(agg);
            }

            void UpdateAggregate(PersonAggregate agg)
            {
                var cmd = new RenamePerson() { Id = agg.Id, Name = "new value" };
                agg.Rename(cmd);
            }

        PersonAggregate CreatePersonAggregate(string id, string name)
        {
            var pa = new PersonAggregate(new PersonAggregateState());
            pa.Create(new CreatePerson() { Id = id, Name = name });
            return pa;
        }

        PersonAggregate CreatePersonAggregate(string id, string name, int numberOfEventsToAdd)
        {
            var pa = CreatePersonAggregate(id, name);
            for (int i = 0; i < numberOfEventsToAdd; i++)
                pa.Rename(new RenamePerson() { Id = id, Name = $"Name {i + 1}" });
            return pa;
        }

        async Task CreateAndStoreAggregate(string id)
        {
            var agg = CreatePersonAggregate(id, "Zvjezdan");
            await Repository.StoreAsync(agg);
        }
    }
}