using EventStore.ClientAPI.Projections;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Starnet.Projections.ES
{
    public interface IJSProjectionsFactory
    {
        public Task CreateProjections();
    }

    public class JSProjectionsFactory : IJSProjectionsFactory
    {
        readonly ILogger Logger;

        ProjectionsManager ProjectionManager;

        public Dictionary<string, string> Projections { get;  set; }

        public JSProjectionsFactory(ILogger logger)
        {
            Projections = new Dictionary<string, string>();
            Logger = logger;
            ProjectionManager = new ProjectionsManager(new LoggerWrapper(Logger), ESConnectionConfig.HttpEndpoint, TimeSpan.FromSeconds(10));
        }

        public async Task CreateProjections()
        {
            List<ProjectionDetails> projections = await ProjectionManager.ListAllAsync(ESConnectionConfig.UserCredentials);
            var projectionNames = (from p in projections select p.Name).ToList();
            var newProjections = GetNewProjectionNames(projectionNames);
            foreach (var kv in newProjections)
                await ProjectionManager.CreateContinuousAsync(kv.Key, kv.Value, ESConnectionConfig.UserCredentials);
        }

            Dictionary<string, string> GetNewProjectionNames(List<string> existing)
            {
                return (from kv in Projections where !existing.Contains(kv.Key) select kv).ToDictionary(kv=> kv.Key, kv=>kv.Value);
            }
    }

    public class LoggerWrapper : EventStore.ClientAPI.ILogger
    {
        ILogger Logger;

        public LoggerWrapper(ILogger logger)
        {
            Logger = logger;
        }

        public void Debug(string format, params object[] args)
        {
            Logger.LogDebug(format, args);
        }

        public void Debug(Exception ex, string format, params object[] args)
        {
            Logger.LogDebug(ex, format, args);
        }

        public void Error(string format, params object[] args)
        {
            Logger.LogError(format, args);
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            Logger.LogError(ex, format, args);
        }

        public void Info(string format, params object[] args)
        {
            Logger.LogInformation(format, args);
        }

        public void Info(Exception ex, string format, params object[] args)
        {
            Logger.LogInformation(ex, format, args);
        }
    }
}