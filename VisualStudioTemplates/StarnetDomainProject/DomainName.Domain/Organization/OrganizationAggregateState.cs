using $ext_projectname$.PL;
using $ext_projectname$.PL.Events;
using Starnet.Aggregates;

namespace $safeprojectname$.Organization
{
    public class OrganizationAggregateState : AggregateState
    {
        internal string Name { get; set; }
        internal Address Address { get; set; }

        protected override void DelegateWhenToConcreteClass(object ev)
        {
            When((dynamic)ev);
        }

        void When(OrganizationRegistered e)
        {
            Id = e.Id;
            Name = e.Name;
            Address = e.Address;
        }
    }
}