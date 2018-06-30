using $ext_projectname$.PL.Commands;
using $ext_projectname$.PL.Events;
using Starnet.Aggregates;


namespace $safeprojectname$.Company
{
    public class CompanyAggregate : Aggregate
    {
        private CompanyAggregateState CompanyState;

        public CompanyAggregate(CompanyAggregateState state) : base(state)
        {
            CompanyState = state;
        }

        internal void RegisterCompany(RegisterCompany c)
        {
            var e = new CompanyRegistered() { Id = c.Id, IssuedBy = c.IssuedBy, TimeIssued = c.TimeIssued,
                Name = c.Name,
                Address = c.Address,
                VATId = c.VATId
            };
            Apply(e);
        }
    }
}
