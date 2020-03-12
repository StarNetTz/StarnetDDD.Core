namespace Starnet.Aggregates.Tests
{
    internal class PersonAggregate : Aggregate
    {
        PersonAggregateState State;

        public PersonAggregate(PersonAggregateState state) : base(state)
        {
            State = state;
        }

        internal void Create(RegisterPerson cmd)
        {
            var e = new PersonRegistered() { Id = cmd.Id, Name = cmd.Name };
            Apply(e);
            PublishedEvents.Add(e);
        }

        internal void Rename(RenamePerson cmd)
        {
            if (string.IsNullOrEmpty(cmd.Name))
                throw DomainError.Named("InvalidPersonName", "Name cannot be null, empty or whitespace!");
            var e = new PersonRenamed() { Id = cmd.Id, Name = cmd.Name };
            Apply(e);
        }
    }
}