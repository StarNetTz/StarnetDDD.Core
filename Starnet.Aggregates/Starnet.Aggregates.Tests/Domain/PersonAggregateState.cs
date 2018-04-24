using System;

namespace Starnet.Aggregates.Tests
{
    internal class PersonAggregateState : AggregateState
    {

        private string Name { get; set; }

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
}