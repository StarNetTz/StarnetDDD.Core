using $ext_projectname$.WebApi.ServiceInterface;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Testing;
using ServiceStack.Validation;
using System;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    [TestFixture]
    public class CommandServiceTestBase<T> where T : Service
    {
        protected T Service;
        protected ServiceStackHost AppHost;

        [OneTimeSetUp]
        public void ConfigureAppHost()
        {
            AppHost = new BasicAppHost(typeof(OrganizationService).Assembly)
            {
                TestMode = true,
                ConfigureContainer = container =>
                {
                    container.RegisterAutoWiredAs<StubMessageBus, IMessageBus>();
                    container.RegisterAutoWiredAs<StubTimeProvider, ITimeProvider>();
                    container.Register<IAuthSession>(c => new AuthUserSession
                    {
                        Email = "mensad@mail.com"
                    });
                    container.RegisterValidators(typeof(AddressValidator).Assembly);
                }
            };
            AppHost.Plugins.Add(new ValidationFeature());
            AppHost.Init();
            Service = AppHost.Container.Resolve<T>();
            Service.Request = new MockHttpRequest();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => AppHost.Dispose();

        public class StubMessageBus : IMessageBus
        {
            public Task Send(object message)
            {
                return Task.CompletedTask;
            }
        }

        public class StubTimeProvider : ITimeProvider
        {
            public DateTime GetUtcTime()
            {
                return DateTime.MinValue;
            }
        }
    }
}