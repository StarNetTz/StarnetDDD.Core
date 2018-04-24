using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starnet.Aggregates.Testing
{
    //ncrunch: no coverage start
    public abstract class ApplicationServiceSpecification<TCommand, TEvent> : IListSpecifications<TCommand, TEvent>
    {
        bool ThenWasCalled = false;
        readonly List<TEvent> GivenEvents = new List<TEvent>();
        TCommand WhenCommand;
        readonly List<TEvent> ThenEvents = new List<TEvent>();

        protected abstract Task<TEvent[]> ExecuteCommand(TEvent[] store, TCommand cmd);

        public void Given(params TEvent[] g)
        {
            GivenEvents.AddRange(g);
        }

        protected void When(TCommand command)
        {
            WhenCommand = command;
        }

        public async Task Expect(params TEvent[] g)
        {
            ThenWasCalled = true;
            ThenEvents.AddRange(g);

            TEvent[] actual;
            var givenEvents = GivenEvents.ToArray();
            actual = await ExecuteCommand(givenEvents, WhenCommand);

            var results = CompareAssert(ThenEvents.ToArray(), actual).ToArray();

            PrintResults(results);

            if (results.Any(r => r.Failure != null))
                Assert.Fail("Specification failed");
        }

        protected static void PrintResults(ICollection<ExpectResult> exs)
        {
            var results = exs.ToArray();
            var failures = results.Where(f => f.Failure != null).ToArray();
            if (!failures.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Results: [Passed]");
                return;
            }
            Console.WriteLine();
            Console.WriteLine("Results: [Failed]");

            for (int i = 0; i < results.Length; i++)
            {
                PrintAdjusted(string.Format("  {0}. ", (i + 1)), results[i].Expectation);
                PrintAdjusted("     ", results[i].Failure ?? "PASS");
            }
        }

        static void PrintAdjusted(string adj, string text)
        {
            Console.Write(GetAdjusted(adj, text));
        }

        public async Task ExpectError(string name)
        {
            ThenWasCalled = true;
            try
            {
                await ExecuteCommand(GivenEvents.ToArray(), WhenCommand);
            }
            catch (DomainError e)
            {
                if (e.Name.Equals(name))
                    return;
            }
            Assert.Fail("Specification failed");
        }

        protected static IEnumerable<ExpectResult> CompareAssert(TEvent[] expected, TEvent[] actual)
        {
            var max = Math.Max(expected.Length, actual.Length);

            for (int i = 0; i < max; i++)
            {
                var ex = expected.Skip(i).FirstOrDefault();
                var ac = actual.Skip(i).FirstOrDefault();

                var expectedString = ex == null ? "No event expected" : ex.ToString();
                var actualString = ac == null ? "No event actually" : ac.ToString();

                var result = new ExpectResult { Expectation = expectedString };

                var realDiff = ObjectComparer.FindDifferences(ex, ac);
                if (!string.IsNullOrEmpty(realDiff))
                {
                    var stringRepresentationsDiffer = expectedString != actualString;

                    result.Failure = stringRepresentationsDiffer ?
                        GetAdjusted("Was:  ", actualString) :
                        GetAdjusted("Diff: ", realDiff);
                }

                yield return result;
            }
        }

        public static string GetAdjusted(string adj, string text)
        {
            var first = true;
            var builder = new StringBuilder();
            foreach (var s in text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                builder.Append(first ? adj : new string(' ', adj.Length));
                builder.AppendLine(s);
                first = false;
            }
            return builder.ToString();
        }

        [SetUp]
        public void SetUpSpecification()
        {
            WhenCommand = default(TCommand);
            GivenEvents.Clear();
            ThenEvents.Clear();
        }

        [TearDown]
        public void TeardownSpecification()
        {
            if (!ThenWasCalled)
                Assert.Fail("THEN was not called from the unit test");
        }

        public IEnumerable<SpecificationInfo<TCommand, TEvent>> ListSpecifications()
        {
            throw new NotImplementedException();
        }
    }
    //ncrunch: no coverage end
}
