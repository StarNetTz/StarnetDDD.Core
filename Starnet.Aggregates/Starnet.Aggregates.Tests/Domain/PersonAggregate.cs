using System.Collections.Generic;

namespace Starnet.Aggregates.Tests
{
    internal class PersonAggregate : Aggregate
    {
        PersonAggregateState State;

        public PersonAggregate(PersonAggregateState state) : base(state)
        {
            State = state;
        }

        internal List<object> Create(CreatePerson cmd)
        {
            var publishedEvents = new List<object>();
            var e = new PersonCreated() { Id = cmd.Id, Name = cmd.Name };
            Apply(e);
           
            publishedEvents.Add(e);
            return publishedEvents;
        }

        internal List<object> Rename(RenamePerson cmd)
        {
            if (string.IsNullOrEmpty(cmd.Name))
                throw DomainError.Named("InvalidPersonName", "Name cannot be null, empty or whitespace!");
            var e = new PersonRenamed() { Id = cmd.Id, Name = cmd.Name };
            Apply(e);
            return new List<object>();
        }
    }
}