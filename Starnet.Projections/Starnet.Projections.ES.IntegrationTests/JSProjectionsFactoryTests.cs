using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NUnit.Framework;
using SimpleInjector;
using System.Threading.Tasks;

namespace Starnet.Projections.ES.IntegrationTests
{
    class JSProjectionsFactoryTests
    {
        [Test]
        public async Task CanCreateProjectionUsingSimpleInjector()
        {
            var container = CreateDIContainer();
            var fact = container.GetInstance<JSProjectionsFactory>();
            fact.Projections.Add("AssociationsOverviewWithNlog", "fromStreams('$ce-Competitions').when({CompetitionAdded: function(s,e){linkTo('cp-AssociationsOverview', e);return s;},CompetitionRenamed: function(s,e){linkTo('cp-AssociationsOverview', e);return s;},CompetitionsAssociationChanged: function(s,e){linkTo('cp-AssociationsOverview', e);return s;},CompetitionsRankChanged: function(s,e){linkTo('cp-AssociationsOverview', e);return s;}})");
            await fact.CreateProjections();
        }

            static Container CreateDIContainer()
            {
                var container = new Container();
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                });

                LogManager.LoadConfiguration("nlog.config");
                container.Register(() => loggerFactory, Lifestyle.Singleton);
                container.Register<JSProjectionsFactory>();
                container.RegisterSingleton(typeof(ILogger<>), typeof(Logger<>));
                container.Verify();
                return container;
            }
    }
}