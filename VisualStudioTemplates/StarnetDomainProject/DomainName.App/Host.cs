using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using NServiceBus.Logging;
using Starnet.Aggregates.ES;
using System;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    class Host
    {
        static readonly ILog log = LogManager.GetLogger<Host>();

        IEndpointInstance endpoint;
        IConfiguration Configuration;

        public string EndpointName { get; set; }

        public Host()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            EndpointName = Configuration["NSBus:EndpointName"];
        }

        public async Task Start()
        {
            try
            {
                var endpointConfiguration = CreateEndpointConfiguration(Configuration);
                endpoint = await Endpoint.Start(endpointConfiguration);
                Console.WriteLine("Use 'docker-compose down' to stop containers.");
            }
            catch (Exception ex)
            {
                FailFast("Failed to start.", ex);
            }
        }

        EndpointConfiguration CreateEndpointConfiguration(IConfiguration config)
        {
            var endpointConfiguration = new EndpointConfiguration(config["NSBus:EndpointName"]);
            endpointConfiguration.RegisterComponents(reg =>
            {
                reg.ConfigureComponent<ESAggregateRepository>(CreateEventStoreAggregateRepository, DependencyLifecycle.SingleInstance);
                RegisterDomainApplicationservices(reg);
            });
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();

            transport.ConnectionString(config["RabbitMQ:ConnectionString"]);


            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(
                type =>
                {
                    return (
                    (type.Namespace == "$ext_projectname$.PL.Commands")
                    );
                });

            endpointConfiguration.EnableInstallers();
            return endpointConfiguration;
        }

        static void RegisterDomainApplicationservices(NServiceBus.ObjectBuilder.IConfigureComponents reg)
        {
            foreach (var type in ApplicationServiceExtractor.GetApplicationServiceClassTypes())
                reg.ConfigureComponent(type, DependencyLifecycle.SingleInstance);
        }

        ESAggregateRepository CreateEventStoreAggregateRepository()
        {
            var uri = new Uri(Configuration["EventStore:Uri"]);
            var Connection = EventStoreConnection.Create(uri);
            Connection.ConnectAsync().Wait();
            var Repository = new ESAggregateRepository(Connection);
            return Repository;
        }

        public async Task Stop()
        {
            try
            {
                await endpoint?.Stop();
            }
            catch (Exception ex)
            {
                FailFast("Failed to stop correctly.", ex);
            }
        }

        async Task OnCriticalError(ICriticalErrorContext context)
        {
            try
            {
                await context.Stop();
            }
            finally
            {
                FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }
        }

        void FailFast(string message, Exception exception)
        {
            try
            {
                log.Fatal(message, exception);
            }
            finally
            {
                Environment.FailFast(message, exception);
            }
        }
    }
}
