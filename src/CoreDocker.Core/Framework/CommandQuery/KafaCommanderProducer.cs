using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Mappers;
using Kafka.Public;
using Serilog;
using ILogger = Kafka.Public.ILogger;


namespace CoreDocker.Core.Framework.CommandQuery
{
    public class KafaCommanderProducer : ICommander, IDisposable
    {
        private static readonly Serilog.ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()!.DeclaringType);
        
        private Lazy<ClusterClient> _eventProducer;

        public KafaCommanderProducer(string host = "localhost:9092")
        {
            _eventProducer = new Lazy<ClusterClient>(() => new ClusterClient(new Configuration {Seeds = host}, LoggingAdapt.From(_log.ForContext(typeof(ClusterClient)))));
        }

        private class LoggingAdapt : ILogger
        {
            public static LoggingAdapt From(Serilog.ILogger log)
            {
                return new LoggingAdapt(log);
            }

            private readonly Serilog.ILogger _log;

            private LoggingAdapt(Serilog.ILogger log)
            {
                _log = log;
            }

            #region Implementation of ILogger

            public void LogInformation(string message)
            {
                _log.Information(message);
            }

            public void LogWarning(string message)
            {
                _log.Warning(message);
            }

            public void LogError(string message)
            {
                _log.Error(message);
            }

            public void LogDebug(string message)
            {
                _log.Debug(message);
            }

            #endregion
        }

        #region Implementation of ICommander

        public async Task Notify<T>(T notificationRequest, CancellationToken cancellationToken) where T : CommandNotificationBase
        {
            _eventProducer.Value.Produce("core.docker.events", notificationRequest);
        }

        public async Task<CommandResult> Execute<T>(T commandRequest, CancellationToken cancellationToken) where T : CommandRequestBase
        {
            _eventProducer.Value.Produce("core.docker.commands", commandRequest);
            return commandRequest.ToCommandResult();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
           if (_eventProducer.IsValueCreated) _eventProducer.Value.Dispose();
        }

        #endregion
    }
}