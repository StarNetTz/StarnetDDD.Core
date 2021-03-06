﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Projections
{
    public interface IProjection
    {
        string Name { get; set; }
        ISubscription Subscription { get; set; }
        IEnumerable<IHandler> Handlers { get; set; }
        Checkpoint Checkpoint { get; set; }
        Task Project(object e, long c);
        Task Start();
    }
}
