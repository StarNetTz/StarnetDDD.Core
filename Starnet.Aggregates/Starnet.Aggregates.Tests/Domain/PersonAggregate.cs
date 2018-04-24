using System;

namespace Starnet.Aggregates.Tests
{
    internal class PersonAggregate : Aggregate
    {
        private PersonAggregateState State;

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
            if (string.IsNullOrEmpty(cmd.Name))
                throw DomainError.Named("InvalidPersonName", "Name cannot be null, empty or whitespace!");
            Apply(new PersonRenamed() { Id = cmd.Id, Name = cmd.Name });
        }
    }
}