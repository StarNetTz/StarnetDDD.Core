using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using System.Security.Cryptography.X509Certificates;

namespace $safeprojectname$
{
    public class RavenConfig
    {
        public string[] Urls { get; set; }
        public string DatabaseName { get; set; }
        public string CertificateFilePath { get; set; }
        public string CertificateFilePassword { get; set; }

        public static RavenConfig FromConfiguration(IConfiguration conf)
        {
            return new RavenConfig { Urls = conf["RavenDb:Urls"].Split(';'), CertificateFilePassword = conf["RavenDb:CertificatePassword"], CertificateFilePath = conf["RavenDb:CertificatePath"], DatabaseName = conf["RavenDb:DatabaseName"] };
        }
    }

    public class RavenDocumentStoreFactory
    {
        public IDocumentStore CreateDocumentStore(RavenConfig conf)
        {
            var store = new DocumentStore { Urls = conf.Urls };
            if (!string.IsNullOrWhiteSpace(conf.CertificateFilePath))
                store.Certificate = new X509Certificate2(conf.CertificateFilePath, conf.CertificateFilePassword);
            store.Database = conf.DatabaseName;
            store.Initialize();
            IndexCreation.CreateIndexes(typeof(OrganizationSmartSearchQuery).Assembly, store);
            return store;
        }
    }
}