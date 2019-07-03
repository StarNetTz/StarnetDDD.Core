using $ext_projectname$.PL.Commands;
using Starnet.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.Organization
{
    public interface IOrganizationInteractor : IInteractor { }

    public class OrganizationInteractor : Interactor, IOrganizationInteractor
    {
        readonly IAggregateRepository AggRepository;

        public OrganizationInteractor(IAggregateRepository aggRepository)
        {
            AggRepository = aggRepository;
        }

        async Task CreateAgg(string id, Action<OrganizationAggregate> usingThisMethod)
        {
            var agg = await AggRepository.GetAsync<OrganizationAggregate>(id);
            if (agg != null)
                throw DomainError.Named("OrganizationAlreadyExists", string.Empty);
            agg = new OrganizationAggregate(new OrganizationAggregateState());
            usingThisMethod(agg);
			PublishedEvents = agg.PublishedEvents;
            await AggRepository.StoreAsync(agg);
        }

        public override async Task Execute(object command)
        {
            await When((dynamic)command);
        }

        private async Task When(RegisterOrganization c)
        {
            await CreateAgg(c.Id, agg => agg.RegisterOrganization(c));
        }
    }
}