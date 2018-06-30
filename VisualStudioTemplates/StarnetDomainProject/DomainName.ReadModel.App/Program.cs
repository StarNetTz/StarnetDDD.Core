using $ext_projectname$.ReadModel.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SimpleInjector;
using Starnet.Projections;
using Starnet.Projections.ES;
using Starnet.Projections.RavenDb;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    class Program
    {
        static SemaphoreSlim semaphore = new SemaphoreSlim(0);
        static IConfiguration Configuration;
        static Container Container;

        async static Task Main(string[] args)
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            Container = CreateDIContainer();
            Console.CancelKeyPress += CancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            var srv = Container.GetInstance<ServiceInstance>();
            srv.Start(Container);
            await Console.Out.WriteLineAsync("Press Ctrl+C to exit...");
            await semaphore.WaitAsync();
        }

        static void CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            semaphore.Release();
        }

        static void ProcessExit(object sender, EventArgs e)
        {
            semaphore.Release();
        }

        static Container CreateDIContainer()
        {
            var Container = new Container();

            ConfigureLogging(Container);
            Container.Register<ServiceInstance>();
            Container.Register<INoSqlStore, RavenDbProjectionsStore>(Lifestyle.Singleton);
            Container.Register<ISqlStore, RavenDbProjectionsStore>(Lifestyle.Singleton);
            Container.Register<ICheckpointReader, RavenDbCheckpointReader>();
            Container.Register<ICheckpointWriter, RavenDbCheckpointWriter>();
            ConfigureRavenDb(Container);

            Container.Register<IHandlerFactory, DIHandlerFactory>();
            Container.Register<ISubscriptionFactory, ESSubscriptionFactory>();
            Container.Register<IProjectionsFactory, ProjectionsFactory>();
            
            Container.Verify();

            return Container;
        }

            static void ConfigureLogging(Container Container)
            {
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                NLog.LogManager.LoadConfiguration("nlog.config");
                Container.Register<ILoggerFactory>(() => loggerFactory, Lifestyle.Singleton);
                Container.RegisterConditional(typeof(ILogger), c => typeof(Logger<>).MakeGenericType(c.Consumer.ImplementationType), Lifestyle.Singleton, c => true);
            }

            static void ConfigureRavenDb(Container Container)
            {
                var docStore = new RavenDocumentStoreFactory().CreateDocumentStore(RavenConfig.FromConfiguration(Configuration));
                Container.Register(() => docStore, Lifestyle.Singleton);
            }    
    }
}
