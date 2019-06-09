using $ext_projectname$.WebApi.ServiceInterface;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class NSBus : IMessageBus
    {
        public async Task Send(object message)
        {
            await NSBusSingleton.$ext_projectname$AppEndpointInstance.Send(message);
        }
    }

    class NSBusSingleton
    {
        public static IEndpointInstance $ext_projectname$AppEndpointInstance;

        static NSBusSingleton()
        {

            $ext_projectname$AppEndpointInstance = Endpoint.Start(CreateEndpointConfiguration()).GetAwaiter().GetResult();
        }

        static EndpointConfiguration CreateEndpointConfiguration()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            var endpointConfiguration = new EndpointConfiguration(config["NSBus:EndpointName"]);

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();

            transport.ConnectionString(config["RabbitMQ:ConnectionString"]);

            var routing = transport.Routing();
            routing.RouteToEndpoint(
                assembly: typeof(PL.Commands.RegisterOrganization).Assembly,
                destination: config["NSBus:$ext_projectname$AppEndpointName"]);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(
                type =>
                {
                    return (
                    (type.Namespace == "$ext_projectname$.PL.Commands")
                    );
                });

            endpointConfiguration.SendOnly();
            endpointConfiguration.EnableInstallers();
            return endpointConfiguration;
        }
    }
}
