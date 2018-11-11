using $ext_projectname$.PL.Commands;
using $ext_projectname$.PL.Events;
using Starnet.Aggregates;


namespace $safeprojectname$.Organization
{
    public class OrganizationAggregate : Aggregate
    {
        private OrganizationAggregateState OrganizationState;

        public OrganizationAggregate(OrganizationAggregateState state) : base(state)
        {
            OrganizationState = state;
        }

        internal void RegisterOrganization(RegisterOrganization c)
        {
            var e = new OrganizationRegistered()
            {
                Id = c.Id,
                IssuedBy = c.IssuedBy,
                TimeIssued = c.TimeIssued,
                Name = c.Name,
                Address = c.Address
            };
            Apply(e);
        }
    }
}
