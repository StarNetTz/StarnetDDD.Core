using $ext_projectname$.PL.Commands;
using Starnet.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.Company
{
    public interface ICompanyApplicationService : IApplicationService { }

    public class CompanyApplicationService : ICompanyApplicationService
    {
        readonly IAggregateRepository AggRepository;

        public CompanyApplicationService(IAggregateRepository aggRepository)
        {
            AggRepository = aggRepository;
        }

        async Task CreateAgg(string id, Action<CompanyAggregate> usingThisMethod)
        {
            var agg = await AggRepository.GetAsync<CompanyAggregate>(id);
            if (agg != null)
                throw DomainError.Named("CompanyAlreadyExists", string.Empty);
            agg = new CompanyAggregate(new CompanyAggregateState());
            usingThisMethod(agg);
            await AggRepository.StoreAsync(agg);
        }

        public async Task Execute(object command)
        {
            await When((dynamic)command);
        }

        private async Task When(RegisterCompany c)
        {
            await CreateAgg(c.Id, agg => agg.RegisterCompany(c));
        }
    }
}