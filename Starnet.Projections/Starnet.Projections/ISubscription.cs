using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Projections
{
    public interface ISubscription
    {
        string StreamName { get; set; }
        Func<object, long, Task> EventAppearedCallback { get; set; }
        Task Start(long fromCheckpoint);
    }
}
