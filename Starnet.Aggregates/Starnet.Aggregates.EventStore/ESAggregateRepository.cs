using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starnet.Aggregates.ES
{
    public class ESAggregateRepository : IAggregateRepository
    {
        const string EventClrTypeHeader = "EventClrTypeName";
        const string AggregateClrTypeHeader = "AggregateClrTypeName";
        const string CommitIdHeader = "CommitId";
        const int WritePageSize = 500;
        const int ReadPageSize = 500;

        readonly IEventStoreConnection EventStoreConnection;
        readonly JsonSerializerSettings SerializerSettings;

        public ESAggregateRepository(IEventStoreConnection eventStoreConnection)
        {
            SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            EventStoreConnection = eventStoreConnection;
        }
       
        public async Task StoreAsync(IAggregate aggregate)
        {
            await TrySaveAggregate(aggregate);
        }

        async Task TrySaveAggregate(IAggregate aggregate)
        {
            try
            {
                await SaveAggregate(aggregate, Guid.NewGuid(), (d) => { });
            }
            catch (WrongExpectedVersionException ex)
            {
                throw new ConcurrencyException(ex.Message, ex);
            }
        }

        async Task SaveAggregate(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            var commitHeaders = new Dictionary<string, object>
            {
                {CommitIdHeader, commitId},
                {AggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName}
            };
            updateHeaders(commitHeaders);

            var streamName = aggregate.Id;
            var newEvents = aggregate.Changes.Cast<object>().ToList();
            var originalVersion = aggregate.Version - newEvents.Count;
            var expectedVersion = originalVersion == 0 ? ExpectedVersion.NoStream : originalVersion - 1;
            var eventsToSave = newEvents.Select(e => ToEventData(e, commitHeaders)).ToList();

            if (eventsToSave.Count < WritePageSize)
                await EventStoreConnection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave);
            else
            {
                var transaction = await EventStoreConnection.StartTransactionAsync(streamName, expectedVersion);
                var position = 0;
                while (position < eventsToSave.Count)
                {
                    var pageEvents = eventsToSave.Skip(position).Take(WritePageSize);
                    await transaction.WriteAsync(pageEvents);
                    position += WritePageSize;
                }
                await transaction.CommitAsync();
            }

            aggregate.Changes.Clear();
        }

            EventData ToEventData(dynamic evnt, IDictionary<string, object> headers)
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));
                var eventHeaders = new Dictionary<string, object>(headers)
                {
                    {
                        EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName
                    }
                };
                var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
                var typeName = evnt.GetType().Name;
                return new EventData(Guid.NewGuid(), typeName, true, data, metadata);
            }

        public Task<TAggregate> GetAsync<TAggregate>(string id) where TAggregate : class, IAggregate
        {
            return GetAsync<TAggregate>(id, int.MaxValue);
        }

        public async Task<TAggregate> GetAsync<TAggregate>(string id, int version) where TAggregate : class, IAggregate
        {
            if (version <= 0)
                throw new InvalidOperationException("Cannot get version <= 0");

            var streamName = id;
            Type aggregateType = typeof(TAggregate);
            var instanceOfState = AggregateStateFactory.CreateStateFor(aggregateType);

            long sliceStart = 0;
            StreamEventsSlice currentSlice;
            do
            {
                int sliceCount = (sliceStart + ReadPageSize <= version) ? ReadPageSize : (int)(version - sliceStart + 1);

                currentSlice = await EventStoreConnection.ReadStreamEventsForwardAsync(streamName, sliceStart, sliceCount, false);

                if (currentSlice.Status == SliceReadStatus.StreamNotFound)
                    break;

                if (currentSlice.Status == SliceReadStatus.StreamDeleted) break;

                sliceStart = currentSlice.NextEventNumber;

                foreach (var evnt in currentSlice.Events)
                {
                    instanceOfState.Mutate(DeserializeEvent(evnt.OriginalEvent.Metadata, evnt.OriginalEvent.Data));
                    if (instanceOfState.Version == version)
                        return Activator.CreateInstance(aggregateType, instanceOfState) as TAggregate;
                }
            } while (version >= currentSlice.NextEventNumber && !currentSlice.IsEndOfStream);

            if (instanceOfState.Version == 0)
                return null;
            return Activator.CreateInstance(aggregateType, instanceOfState) as TAggregate;
        }

            object DeserializeEvent(byte[] metadata, byte[] data)
            {
                var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName), SerializerSettings);
            }
    }
}