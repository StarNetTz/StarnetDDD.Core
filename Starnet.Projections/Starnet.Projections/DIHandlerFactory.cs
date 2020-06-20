﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace Starnet.Projections
{
    public class DIHandlerFactory : IHandlerFactory
    {
        IServiceProvider Provider;

        public DIHandlerFactory(IServiceProvider provider)
        {
            Provider = provider;
        }

        public IHandler Create(Type t)
        {
            return Provider.GetRequiredService(t) as IHandler;
        }
    }
}