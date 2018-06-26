using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Starnet.Projections.ES
{
    public class ESSubscription : ISubscription
    {
        public string StreamName { get; set; }

        public Func<object, long, Task> EventAppearedCallback { get; set; }

        const string EventClrTypeHeader = "EventClrTypeName";
        readonly IEventStoreConnection Connection;

        EventStoreStreamCatchUpSubscription Subscription = null;
       
        readonly JsonSerializerSettings SerializerSettings;

        public ESSubscription(IEventStoreConnection connection)
        {
            Connection = connection;
            SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        object DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
            var jsonString = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject(jsonString, Type.GetType((string)eventClrTypeName), SerializerSettings);
        }

        public async Task EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent @event)
        {
            var ev = DeserializeEvent(@event.Event.Metadata, @event.Event.Data);
            await EventAppearedCallback(ev, @event.OriginalEventNumber + 1);
        }

        public async Task Start(long fromCheckpoint)
        {
            await Connection.ConnectAsync();
            CatchUpSubscriptionSettings settings = new CatchUpSubscriptionSettings(500, 500, false, true);
            long? eventstoreCheckpoint = (fromCheckpoint == 0) ? null : (long?)(fromCheckpoint - 1);
            Subscription = Connection.SubscribeToStreamFrom(StreamName, eventstoreCheckpoint, settings, EventAppeared);
        }
    }
}