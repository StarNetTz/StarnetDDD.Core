using Microsoft.Extensions.Configuration;
using EventStore.ClientAPI.SystemData;
using System;
using System.Net;
using System.IO;

namespace Starnet.Projections.ES
{
    public class ESConnectionConfig
    {
        public static UserCredentials UserCredentials { get; set; }
        public static IPEndPoint HttpEndpoint { get; set; }
        public static IPEndPoint TcpEndpoint { get; set; }
        public static IConfiguration Configuration { get; set; }
        public static string ConnectionString { get; set; }

        static ESConnectionConfig()
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
            ConnectionString = Configuration["EventStore:ConnectionString"];
        }
    }
}
