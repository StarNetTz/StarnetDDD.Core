using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Projections
{
    public class Projection : IProjection
    {
        public string Name { get; set; }
        public string SubscriptionStreamName { get; set; }
        public ISubscription Subscription { get; set; }
        public IEnumerable<IHandler> Handlers { get; set; }
        public ICheckpointWriter CheckpointWriter { get; set; }
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public Checkpoint Checkpoint { get; set; }
       
        public async Task Project(object e, long c)
        {
            try
            {
                await HandleEvent(e, c);
            }
            catch (AggregateException ex)
            {
                var trace = $"Projection {Name} on stream {SubscriptionStreamName} failed on checkpoint {c} while trying to project {e.GetType().FullName}";
                Logger.Error(trace);
                Logger.Error(ex);
                throw;
            }
        }

        Task HandleEvent(object e, long c)
        {
            Checkpoint.Value = c;
            Task.WaitAll(StartHandlingTasks(e, c));
            return Task.CompletedTask;
        }

        Task[] StartHandlingTasks(object e, long c)
        {
            List<Task> tasks = new List<Task>();
            foreach (var d in Handlers)
                tasks.Add(d.Handle(e, c));
            tasks.Add(CheckpointWriter.Write(Checkpoint));
            return tasks.ToArray();
        }

        public async Task Start()
        {
            await Subscription.Start(Checkpoint.Value);
        }
    }
}