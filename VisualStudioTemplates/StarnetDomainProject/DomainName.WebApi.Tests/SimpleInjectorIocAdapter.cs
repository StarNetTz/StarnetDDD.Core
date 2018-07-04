using ServiceStack.Configuration;
using SimpleInjector;

namespace $safeprojectname$
{
    public class SimpleInjectorIocAdapter : IContainerAdapter
    {
        private readonly Container container;

        public SimpleInjectorIocAdapter(Container container)
        {
            this.container = container;
        }

        public T Resolve<T>()
        {
            return (T)this.container.GetInstance(typeof(T));
        }

        public T TryResolve<T>()
        {
            var registration = this.container.GetRegistration(typeof(T));
            return registration == null ? default(T) : (T)registration.GetInstance();
        }
    }
}