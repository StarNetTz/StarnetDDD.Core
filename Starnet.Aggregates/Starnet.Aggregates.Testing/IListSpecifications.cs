using System;
using System.Collections.Generic;
using System.Linq;

namespace Starnet.Aggregates.Testing
{
    public interface IListSpecifications<TCommand, TEvent>
    {
        IEnumerable<SpecificationInfo<TCommand, TEvent>> ListSpecifications();
    }
}
