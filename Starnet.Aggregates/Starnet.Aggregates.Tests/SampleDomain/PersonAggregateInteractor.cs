using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Tests
{
    public interface IPersonAggregateInteractor : IInteractor { }

    public class PersonAggregateInteractor : Interactor, IPersonAggregateInteractor
    {
        readonly IAggregateRepository AggRepository;

        public PersonAggregateInteractor(IAggregateRepository aggRepository)
        {
            AggRepository = aggRepository;
        }

        async Task ChangeAgg(string id, Action<PersonAggregate> usingThisMethod)
        {
            var agg = await AggRepository.GetAsync<PersonAggregate>(id);
            usingThisMethod(agg);
            PublishedEvents = agg.PublishedEvents;
            await AggRepository.StoreAsync(agg);
        }

        async Task CreateAgg(string id, Action<PersonAggregate> usingThisMethod)
        {
            var agg = await AggRepository.GetAsync<PersonAggregate>(id);
            if (agg != null)
                throw DomainError.Named("PersonAlreadyRegistered", string.Empty);
            agg = new PersonAggregate(new PersonAggregateState());
            usingThisMethod(agg);
            PublishedEvents = agg.PublishedEvents;
            await AggRepository.StoreAsync(agg);
        }

        public override async Task Execute(object command)
        {
            await When((dynamic)command);
        }

        async Task When(RegisterPerson c)
        {
            await CreateAgg(c.Id, agg => agg.Create(c));
        }

        async Task When(RenamePerson c)
        {
            await ChangeAgg(c.Id, agg => agg.Rename(c));
        }
    }
}
