
namespace Starnet.Aggregates.Tests
{
    public interface ICommand
    {
        string Id { get; }
    }

    public interface IEvent
    {
        string Id { get; }
    }

    public class CreatePerson : ICommand
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class PersonCreated : IEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class RenamePerson : ICommand
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
