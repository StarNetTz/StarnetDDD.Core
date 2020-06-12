using SimpleInjector;
using System;

namespace Starnet.Projections.Testing
{
    public class SimpleInjectorServiceProvider : IServiceProvider
    {
        public Container Container { get; set; }
        public object GetService(Type serviceType)
        {
            return Container.GetInstance(serviceType);
        }
    }
}