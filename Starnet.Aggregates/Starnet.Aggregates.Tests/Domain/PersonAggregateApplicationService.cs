using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    public interface IPersonAggregateApplicationService : IApplicationService { }

    public class PersonAggregateApplicationService : IPersonAggregateApplicationService
    {
        readonly IAggregateRepository AggRepository;

        public List<object> PublishedEvents { get; internal set; }

        public PersonAggregateApplicationService(IAggregateRepository aggRepository)
        {
            AggRepository = aggRepository;
            PublishedEvents = new List<object>();
        }

        async Task ChangeAgg(string id, Action<PersonAggregate> usingThisMethod)
        {
            var agg = await AggRepository.GetAsync<PersonAggregate>(id);
            usingThisMethod(agg);
            await AggRepository.StoreAsync(agg);
        }

        async Task CreateAgg(string id, Action<PersonAggregate> usingThisMethod)
        {
            var agg = await AggRepository.GetAsync<PersonAggregate>(id);
            if (agg != null)
                throw DomainError.Named("AggregateAlreadyExists", string.Empty);
            agg = new PersonAggregate(new PersonAggregateState());
            usingThisMethod(agg);
            await AggRepository.StoreAsync(agg);

        }

        public async Task Execute(object command)
        {
            await When((dynamic)command);
        }

        async Task When(CreatePerson c)
        {
            await CreateAgg(c.Id, agg => agg.Create(c, PublishedEvents));
        }

        async Task When(RenamePerson c)
        {
            await ChangeAgg(c.Id, agg => agg.Rename(c, PublishedEvents));
        }
    }
}
