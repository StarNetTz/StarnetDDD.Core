using System;
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

        internal void Create(CreatePerson cmd, List<object> publishedEvents)
        {
            var e = new PersonCreated() { Id = cmd.Id, Name = cmd.Name };
            Apply(e);
            publishedEvents.Add(e);
        }

        internal void Rename(RenamePerson cmd, List<object> publishedEvents)
        {
            if (string.IsNullOrEmpty(cmd.Name))
                throw DomainError.Named("InvalidPersonName", "Name cannot be null, empty or whitespace!");
            var e = new PersonRenamed() { Id = cmd.Id, Name = cmd.Name };
            Apply(e);
            publishedEvents.Add(e);
        }
    }
}