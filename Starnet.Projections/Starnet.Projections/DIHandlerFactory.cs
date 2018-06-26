using SimpleInjector;
using System;

namespace Starnet.Projections
{
    public class DIHandlerFactory : IHandlerFactory
    {
        readonly Container Container;

        public DIHandlerFactory(Container container)
        {
            Container = container;
        }

        public IHandler Create(Type t)
        {
            return Container.GetInstance(t) as IHandler;
        }
    }
}
