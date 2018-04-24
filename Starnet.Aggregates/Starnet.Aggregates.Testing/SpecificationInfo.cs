using System;
using System.Linq;

namespace Starnet.Aggregates.Testing
{
    public class SpecificationInfo<TCommand, TEvent>
    {
        public string GroupName;

        public string CaseName;

        public TEvent[] Given;

        public TCommand When;

        public TEvent[] Then;
    }
}
