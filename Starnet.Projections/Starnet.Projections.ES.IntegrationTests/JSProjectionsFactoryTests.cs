using Microsoft.Extensions.DependencyInjection;
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
            var container = new Container();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            LogManager.LoadConfiguration("nlog.config");
            container.Register<ILoggerFactory>(() => loggerFactory, Lifestyle.Singleton);

            container.Register<JSProjectionsFactory>();

            container.RegisterConditional( typeof(Microsoft.Extensions.Logging.ILogger),c => typeof(Logger<>).MakeGenericType(c.Consumer.ImplementationType), Lifestyle.Singleton, c => true);
            container.Verify();

            var fact = container.GetInstance<JSProjectionsFactory>();
            fact.Projections.Add("AssociationsOverviewWithNlog", "fromStreams('$ce-Competitions').when({CompetitionAdded: function(s,e){linkTo('cp-AssociationsOverview', e);return s;},CompetitionRenamed: function(s,e){linkTo('cp-AssociationsOverview', e);return s;},CompetitionsAssociationChanged: function(s,e){linkTo('cp-AssociationsOverview', e);return s;},CompetitionsRankChanged: function(s,e){linkTo('cp-AssociationsOverview', e);return s;}})");
            await fact.CreateProjections();
        }
    }
}