﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starnet.Aggregates
{
    public interface IApplicationService
    {
        Task<List<object>> Execute(object command);
    }
}