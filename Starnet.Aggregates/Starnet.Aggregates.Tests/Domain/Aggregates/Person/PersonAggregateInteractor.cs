using Starnet.Aggregates.Tests.Domain.PL.Commands;
using System;
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

        public override async Task Execute(object command)
        {
            await When((dynamic)command);
        }

            async Task When(CreatePerson c)
            {
                await CreateAgg(c.Id, agg => agg.Create(c));
            }

                async Task CreateAgg(string id, Action<PersonAggregate> usingThisMethod)
                {
                    var agg = await AggRepository.GetAsync<PersonAggregate>(id);
                    if (agg != null)
                        throw DomainError.Named("AggregateAlreadyExists", string.Empty);
                    agg = new PersonAggregate(new PersonAggregateState());
                    usingThisMethod(agg);
                    PublishedEvents = agg.PublishedEvents;
                    await AggRepository.StoreAsync(agg);
                }

            async Task When(RenamePerson c)
            {
                await ChangeAgg(c.Id, agg => agg.Rename(c));
            }

                async Task ChangeAgg(string id, Action<PersonAggregate> usingThisMethod)
                {
                    var agg = await AggRepository.GetAsync<PersonAggregate>(id);
                    usingThisMethod(agg);
                    PublishedEvents = agg.PublishedEvents;
                    await AggRepository.StoreAsync(agg);
                }
    }
}
