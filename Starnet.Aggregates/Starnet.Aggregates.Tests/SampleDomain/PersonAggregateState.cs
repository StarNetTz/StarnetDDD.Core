using System;

namespace Starnet.Aggregates.Tests
{
    internal class PersonAggregateState : AggregateState
    {

        string Name { get; set; }

        protected override void DelegateWhenToConcreteClass(object ev)
        {
            When((dynamic)ev);
        }

        void When(PersonRegistered e)
        {
            Id = e.Id;
            Name = e.Name;
        }

        void When(PersonRenamed e)
        {
            Name = e.Name;
        }
    }
}