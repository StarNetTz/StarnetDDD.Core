using EventStore.ClientAPI.SystemData;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace Starnet.Projections
{
    public class EventStoreConnectionSettings
    {
        public static UserCredentials UserCredentials { get; private set; }
        public static IPEndPoint HttpEndpoint { get; private set; }
        public static IPEndPoint TcpEndpoint { get; private set; }

        static Dictionary<string, string> ParseConnectionString(string cnnString)
        {
            var parts = cnnString.Split(';');
            Dictionary<string, string> properties = new Dictionary<string, string>();
            foreach (var part in parts)
            {
                var subParts = part.Split('=');
                properties.Add(subParts[0].ToUpper(), subParts[1]);

            }
            return properties;
        }

        static EventStoreConnectionSettings()
        {
            var cnnString = ConfigurationManager.ConnectionStrings["GetEeventStoreSubscriptionString"].ToString();
            var propertiesDictionary = ParseConnectionString(cnnString);
            UserCredentials = new UserCredentials(propertiesDictionary["USERNAME"], propertiesDictionary["PASSWORD"]);
            var serverAddres = IPAddress.Parse(propertiesDictionary["SERVER"]);
            int httpPort = int.Parse(propertiesDictionary["HTTP_PORT"]);
            int tcpPort = int.Parse(propertiesDictionary["TCP_PORT"]);
            HttpEndpoint = new IPEndPoint(serverAddres, httpPort);
            TcpEndpoint = new IPEndPoint(serverAddres, tcpPort);
        }
    }
}
