using $ext_projectname$.PL;
using $ext_projectname$.ReadModel;
using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace $safeprojectname$
{
    public interface IDatabaseInitializer
    {
        void Initialize();
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        IDocumentStore Store;

        public DatabaseInitializer(IDocumentStore store)
        {
            Store = store;
        }

        public void Initialize()
        {
            CreateTimezonesConfig();
            CreateLanguagesConfig();
            CreateCurrenciesConfig();
            CreateCountriesConfig();
        }

            void CreateTimezonesConfig()
            {
                const string DocumentId = "lookups-timezones";
                List<LookupItem> data = (from t in TimeZoneInfo.GetSystemTimeZones() select new LookupItem { Id = t.Id, Value = t.DisplayName }).ToList();
                CreateConfigDocument(DocumentId, data);
            }

            void CreateLanguagesConfig()
            {
                const string DocumentId = "lookups-languages";
                List<LookupItem> data = new List<LookupItem>() {
                            new LookupItem { Id = "en", Value = "English" },
                            new LookupItem { Id = "bs", Value = "Bosnian" },
                            new LookupItem { Id = "hr", Value = "Croatian" },
                            new LookupItem { Id = "sr", Value = "Serbian" },
                        };

                CreateConfigDocument(DocumentId, data);
            }

            void CreateCurrenciesConfig()
            {
                const string DocumentId = "lookups-currencies";
                List<LookupItem> data = (from c in Starnet.ISO.CurrencyCodes_ISO4217.Codes select new LookupItem { Id = c.ACode, Value = c.Name }).ToList();
                CreateConfigDocument(DocumentId, data);
            }

            void CreateCountriesConfig()
            {
                const string DocumentId = "lookups-countries";
                List<LookupItem> data = (from c in Starnet.ISO.Countries_ISO3166.Countries select new LookupItem { Id = c.A3, Value = c.Name }).ToList();
                CreateConfigDocument(DocumentId, data);
            }

            void CreateConfigDocument(string DocumentId, List<LookupItem> data)
            {
                using (var ses = Store.OpenSession())
                {
                    var doc = ses.Load<Lookup>(DocumentId);
                    if (null != doc)
                        return;
                    doc = new Lookup { Id = DocumentId, Data = data };
                    ses.Store(doc);
                    ses.SaveChanges();
                }
            }     
    }
}