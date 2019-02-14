using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Starnet.Projections.ES
{
    public class ESSubscription : ISubscription
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        long CurrentCheckpoint = 0;

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

        public async Task EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent @event)
        {
            CurrentCheckpoint = @event.OriginalEventNumber;
            var ev = TryDeserializeEvent(@event.Event.Metadata, @event.Event.Data);
            await EventAppearedCallback(ev, @event.OriginalEventNumber + 1);
        }

            object TryDeserializeEvent(byte[] metadata, byte[] data)
            {
                var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
                var jsonString = Encoding.UTF8.GetString(data);
                try
                {
                    return JsonConvert.DeserializeObject(jsonString, Type.GetType((string)eventClrTypeName), SerializerSettings);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to deserialize type: {eventClrTypeName}");
                    Logger.Error(ex);
                    throw;
                }
            }

        public async Task Start(long fromCheckpoint)
        {
            await Connection.ConnectAsync();
            //bool useVerboseMode = false;
            //bool resloveLinkTos = true;
            //Default CatchUpSubscriptionSettings settings = new CatchUpSubscriptionSettings(10000, 500, useVerboseMode, resloveLinkTos);
            long? eventstoreCheckpoint = (fromCheckpoint == 0) ? null : (long?)(fromCheckpoint - 1);
            Subscription = Connection.SubscribeToStreamFrom(StreamName, eventstoreCheckpoint, CatchUpSubscriptionSettings.Default, EventAppeared, LiveProcessingStarted,SubscriptionDropped);
        }

        void LiveProcessingStarted(EventStoreCatchUpSubscription obj)
        {
            Logger.Trace($"LiveProcessingStarted on stream {StreamName}");
        }

        void SubscriptionDropped(EventStoreCatchUpSubscription projection, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {

            Subscription.Stop();

            switch (subscriptionDropReason)
            {
                case SubscriptionDropReason.UserInitiated:
                    Console.WriteLine($"{projection} projection stopped by user.");
                    break;
                case SubscriptionDropReason.SubscribingError:
                case SubscriptionDropReason.ServerError:
                case SubscriptionDropReason.ConnectionClosed:
                case SubscriptionDropReason.CatchUpError:
                case SubscriptionDropReason.ProcessingQueueOverflow:
                    Logger.Error($"{StreamName} projection stopped because of a transient error ({subscriptionDropReason}). ");
                    Logger.Error($"Exception Detail:  {exception}");
                    Logger.Error("Attempting to restart...");
                    Task.Run(() => Start(CurrentCheckpoint));
                    break;
                case SubscriptionDropReason.EventHandlerException:
                    Logger.Error("EventHandlerException");
                    Logger.Error($"Exception Detail:  {exception}");
                    break;
                default:
                    Logger.Error("Your subscription gg");
                    Logger.Error($"Exception Detail:  {exception}");
                    break;
            }
        }

        
    }
}