using System;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Utilities.Helpers;
using MediatR;
using Newtonsoft.Json;
using Serilog;


namespace CoreDocker.Core.Framework.CommandQuery
{
    public class KafaCommanderProducer : ICommander, IDisposable
    {
        public const string TopicCommands = "core.docker.commands";
        public const string TopicEvents = "core.docker.events";
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()!.DeclaringType);
        private ProducerConfig _producerConfig;
        private Lazy<IProducer<Null, string>> _producer;
        private JsonSerializerSettings _jsonSerializerSettings;


        public KafaCommanderProducer(string host = "localhost:9093")
        {
            _producerConfig = new ProducerConfig
            {
                BootstrapServers = host,
                ClientId = Dns.GetHostName(),
            };

            _producer = new Lazy<IProducer<Null, string>>(() =>
                new ProducerBuilder<Null, string>(_producerConfig).Build());
            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }


        #region Implementation of ICommander

        public async Task Notify<T>(T notificationRequest, CancellationToken cancellationToken)
            where T : CommandNotificationBase
        {
           
            await _producer.Value.ProduceAsync(TopicEvents,
                new Message<Null, string> {Value = JsonConvert.SerializeObject(notificationRequest,Formatting.Indented, _jsonSerializerSettings) },
                cancellationToken);
        }

        public async Task<CommandResult> Execute<T>(T commandRequest, CancellationToken cancellationToken)
            where T : CommandRequestBase
        {
            await _producer.Value.ProduceAsync(TopicCommands,
                new Message<Null, string> {Value = JsonConvert.SerializeObject(commandRequest, Formatting.Indented, _jsonSerializerSettings) }, cancellationToken);
            return commandRequest.ToCommandResult();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_producer.IsValueCreated) _producer.Value.Dispose();
        }

        #endregion
    }

    public class KafaCommanderConsumer
    {
        private readonly IMediator _mediatorCommander;
        private readonly ConsumerConfig _config;
        private JsonSerializerSettings _jsonSerializerSettings;
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public KafaCommanderConsumer(IMediator mediator, string host = "localhost:9093"  )
        {
            _mediatorCommander = mediator;
            _config = new ConsumerConfig
            {
                BootstrapServers = host,
                GroupId = "KafaCommander",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }


        #region Implementation of ICommander

        public Task Start(CancellationToken cancellationToken)
        {
            return Task.Run(() => ConsumeAll(cancellationToken), cancellationToken);
        }

        private void ConsumeAll(CancellationToken cancellationToken)
        {
            _log.Information("ConsumeAll");
            using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
            consumer.Subscribe(KafaCommanderProducer.TopicCommands);
            while (!cancellationToken.IsCancellationRequested)
            {
                _log.Information("---------------------------------------------");
                var consumeResult = consumer.Consume(cancellationToken);
                _log.Information("------------------>---------------------------");
                var dump = JsonConvert.DeserializeObject(consumeResult.Message.Value, _jsonSerializerSettings);
                _log.Information("-------------------->-------------------------");

                try
                {
                    _mediatorCommander.Send(dump, cancellationToken).Wait(cancellationToken);
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                }
            }

            consumer.Close();
        }

        #endregion
    }
}
