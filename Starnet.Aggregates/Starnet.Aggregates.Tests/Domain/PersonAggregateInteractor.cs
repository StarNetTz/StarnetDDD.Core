using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    public interface IPersonAggregateInteractor : IInteractor { }

    public class PersonAggregateInteractor : IPersonAggregateInteractor
    {
        readonly IAggregateRepository AggRepository;

        public List<object> PublishedEvents { get; internal set; }

        public PersonAggregateInteractor(IAggregateRepository aggRepository)
        {
            AggRepository = aggRepository;
            PublishedEvents = new List<object>();
        }

        async Task<List<object>> ChangeAgg(string id, Func<PersonAggregate, List<object>> usingThisMethod)
        {
            var agg = await AggRepository.GetAsync<PersonAggregate>(id);
            var publishedEvents = usingThisMethod(agg);
            await AggRepository.StoreAsync(agg);
            return publishedEvents;
        }

        async Task<List<object>> CreateAgg(string id, Func<PersonAggregate, List<object>> usingThisMethod)
        {
            var agg = await AggRepository.GetAsync<PersonAggregate>(id);
            if (agg != null)
                throw DomainError.Named("AggregateAlreadyExists", string.Empty);
            agg = new PersonAggregate(new PersonAggregateState());
            var publishedEvents = usingThisMethod(agg);
            await AggRepository.StoreAsync(agg);
            return publishedEvents;

        }

        public async Task<List<object>> Execute(object command)
        {
            return await When((dynamic)command);
        }

        async Task<List<object>> When(CreatePerson c)
        {
            return await CreateAgg(c.Id, agg => agg.Create(c));
        }

        async Task<List<object>> When(RenamePerson c)
        {
            return await ChangeAgg(c.Id, agg => agg.Rename(c));
        }
    }
}
