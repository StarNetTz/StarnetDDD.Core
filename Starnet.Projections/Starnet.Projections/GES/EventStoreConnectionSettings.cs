using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;

namespace Starnet.Projections
{
    public class EventStoreConnectionSettings
    {
        public static UserCredentials UserCredentials { get; private set; }
        public static IPEndPoint HttpEndpoint { get; private set; }
        public static IPEndPoint TcpEndpoint { get; private set; }
        public static IConfiguration Configuration { get; set; }

        static EventStoreConnectionSettings()
        {
           
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            UserCredentials = new UserCredentials(Configuration["EventStore:Username"], Configuration["EventStore:Password"]);
            var serverAddres = IPAddress.Parse(Configuration["EventStore:Server"]);
            int httpPort = int.Parse(Configuration["EventStore:Http_port"]);
            int tcpPort = int.Parse(Configuration["EventStore:Tcp_port"]);
            HttpEndpoint = new IPEndPoint(serverAddres, httpPort);
            TcpEndpoint = new IPEndPoint(serverAddres, tcpPort);
        }
    }
}
