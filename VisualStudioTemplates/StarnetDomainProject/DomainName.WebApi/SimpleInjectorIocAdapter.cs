using ServiceStack.Configuration;

namespace $safeprojectname$
{
    public class SimpleInjectorIocAdapter : IContainerAdapter
    {
        private readonly SimpleInjector.Container container;

        public SimpleInjectorIocAdapter(SimpleInjector.Container container)
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
