using System;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Starnet.Projections.Tests
{
    public class EventStoreTestDataGenerator
    {
        private const string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const string CommitIdHeader = "CommitId";

        public async Task WriteEventsToStore(int nrOfEvents)
        {
            var cnn = EventStoreConnection.Create(EventStoreConnectionSettings.TcpEndpoint);
            cnn.ConnectAsync().Wait();
            for (int i = 0; i < nrOfEvents; i++)
            {
                var id = $"Match-{Guid.NewGuid()}";
                await WriteEvent(cnn, id, new TestEvent() { Id = id, SomeValue = $"Match name:{Guid.NewGuid()}" });
            }
        }

        public static async Task WriteEvent(IEventStoreConnection cnn, string streamName, params object[] events)
        {
            var commitHeaders = new Dictionary<string, object>
                            {
                                {CommitIdHeader, Guid.NewGuid().ToString()},
                                {AggregateClrTypeHeader, Assembly.GetExecutingAssembly().GetType().AssemblyQualifiedName}
                            };
            var eventsToSave = events.Select(e => ToEventData(e, commitHeaders)).ToList();
            await cnn.AppendToStreamAsync(streamName, -1, eventsToSave);
        }

        private static EventData ToEventData(dynamic evnt, IDictionary<string, object> headers)
        {
            var SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));

            var eventHeaders = new Dictionary<string, object>(headers) {
                {
                    "EventClrTypeName", evnt.GetType().AssemblyQualifiedName
                }
            };
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
            var typeName = evnt.GetType().Name;
            return new EventData(Guid.NewGuid(), typeName, true, data, metadata);
        }
    }
}