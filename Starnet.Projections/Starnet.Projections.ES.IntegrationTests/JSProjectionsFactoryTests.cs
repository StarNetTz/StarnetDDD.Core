using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Starnet.Projections.ES.IntegrationTests
{
    class JSProjectionsFactoryTests
    {
        [Test]
        public async Task CanCreateProjectionUsingSimpleInjector()
        {
            LogManager.LoadConfiguration("nlog.config");
            var provider = CreateServiceProvider();
            var fact = provider.GetRequiredService<IJSProjectionsFactory>();
            fact.AddProjection("AssociationsOverviewWithNlog", "fromStreams('$ce-Competitions').when({CompetitionAdded: function(s,e){linkTo('cp-AssociationsOverview', e);return s;},CompetitionRenamed: function(s,e){linkTo('cp-AssociationsOverview', e);return s;},CompetitionsAssociationChanged: function(s,e){linkTo('cp-AssociationsOverview', e);return s;},CompetitionsRankChanged: function(s,e){linkTo('cp-AssociationsOverview', e);return s;}})");
            await fact.CreateProjections();
        }

            IServiceProvider CreateServiceProvider()
            {
                var services = new ServiceCollection();
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                });
                services.AddSingleton(loggerFactory);

                services.AddTransient(typeof(ILogger<>), typeof(Logger<>));
                services.AddTransient<IJSProjectionsFactory, JSProjectionsFactory>();
                return services.BuildServiceProvider();
            }
    }
}