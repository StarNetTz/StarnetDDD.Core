using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Starnet.Projections
{
    public class GESSubscription : ISubscription
    {
        public string StreamName { get; set; }

        public Func<object, long, Task> EventAppearedCallback { get; set; }

        private const string EventClrTypeHeader = "EventClrTypeName";
        readonly IEventStoreConnection Connection;
      
        EventStoreStreamCatchUpSubscription Subscription = null;
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly JsonSerializerSettings SerializerSettings;

        static GESSubscription()
        {
            SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        public GESSubscription(IEventStoreConnection connection)
        {
            Connection = connection;
        }

        private static object DeserializeEvent(byte[] metadata, byte[] data)
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

        public void SubscriptionDropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
        {
            Console.WriteLine($"{DateTime.Now} Subscription {subscription.SubscriptionName} dropped. Reason: {reason}.");
            if (ex != null)
            {
                Console.WriteLine($"Exception StackTrace: {ex.StackTrace} \r\n Exception Message: {ex.Message}");
                Logger.Info(ex);
              
            }

            Start(0).Wait();
        }

        public async Task Start(long fromCheckpoint)
        {
            await Connection.ConnectAsync();
            CatchUpSubscriptionSettings settings = new CatchUpSubscriptionSettings(500, 500, false, true);
            long? eventstoreCheckpoint = (fromCheckpoint == 0) ? null : (long?)(fromCheckpoint - 1);
            Subscription = Connection.SubscribeToStreamFrom(StreamName, eventstoreCheckpoint, settings, EventAppeared, null, SubscriptionDropped);
        }
    }
}
