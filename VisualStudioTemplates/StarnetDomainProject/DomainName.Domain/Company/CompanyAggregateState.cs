using $ext_projectname$.PL;
using $ext_projectname$.PL.Events;
using Starnet.Aggregates;

namespace $safeprojectname$.Company
{
    public class CompanyAggregateState : AggregateState
    {
        string Name { get; set; }
        Address Address { get; set; }
        string VATId { get; set; }


        protected override void DelegateWhenToConcreteClass(object ev)
        {
            When((dynamic)ev);
        }

        private void When(CompanyRegistered e)
        {
            Id = e.Id;
            Name = e.Name;
            Address = e.Address;
            VATId = e.VATId;
        }
    }
}