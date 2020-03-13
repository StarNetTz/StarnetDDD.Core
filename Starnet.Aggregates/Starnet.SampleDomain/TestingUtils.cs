using Starnet.Aggregates;
using System;
using System.Threading.Tasks;

namespace Starnet.SampleDomain
{
    public class TestingUtils
    {
        public static async Task UpdateOutOfSession(string aggId, IAggregateRepository rep)
        {
            var agg = await rep.GetAsync<PersonAggregate>(aggId);
            agg.Rename(new RenamePerson() { Id = aggId, Name = "Renamed out of session" });
            await rep.StoreAsync(agg);
        }

        public static void Rename(PersonAggregate agg, string newName)
        {
            agg.Rename(new RenamePerson { Id = agg.Id, Name = newName });
        }
    }
}
