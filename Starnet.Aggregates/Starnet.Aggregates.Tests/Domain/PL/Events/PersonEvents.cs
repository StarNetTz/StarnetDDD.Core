
namespace Starnet.Aggregates.Tests.Domain.PL.Events
{
    public class PersonCreated : IEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class PersonRenamed : IEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
