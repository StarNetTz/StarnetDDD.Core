using Microsoft.Extensions.Configuration;
using System;

namespace Starnet.Projections.RavenDb.IntegrationTests
{
    public class RavenConfig
    {
        public string[] Urls { get; set; }
        public string DatabaseName { get; set; }
        public string CertificateFilePath { get; set; }
        public string CertificateFilePassword { get; set; }

        public static RavenConfig FromConfiguration(IConfiguration conf)
        {
            return new RavenConfig
            {
                Urls = conf["RavenDb:Urls"].Split(';'),
                CertificateFilePassword = conf["RavenDb:CertificatePassword"],
                CertificateFilePath = conf["RavenDb:CertificatePath"],
                DatabaseName = conf["RavenDb:DatabaseName"]
            };
        }
    }
}
