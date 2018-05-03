namespace Starnet.Aggregates.EventStore.Tests
{
    internal class PersonAggregate : Aggregate
    {
        PersonAggregateState State;

        public PersonAggregate(PersonAggregateState state) : base(state)
        {
            State = state;
        }

        internal void Create(CreatePerson cmd)
        {
            Apply(new PersonCreated() { Id = cmd.Id, Name = cmd.Name });
        }

        internal void Rename(RenamePerson cmd)
        {
            Apply(new PersonRenamed() { Id = cmd.Id, Name = cmd.Name });
        }
    }

    internal class PersonAggregateState : AggregateState
    {

        string Name { get; set; }

        protected override void DelegateWhenToConcreteClass(object ev)
        {
            When((dynamic)ev);
        }

        void When(PersonCreated e)
        {
            Id = e.Id;
            Name = e.Name;
        }

        void When(PersonRenamed e)
        {
            Name = e.Name;
        }
    }

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
