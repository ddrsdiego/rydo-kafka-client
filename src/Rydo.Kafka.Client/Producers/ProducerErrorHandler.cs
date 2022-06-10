namespace Rydo.Kafka.Client.Producers
{
    using System;
    using Confluent.Kafka;
    using Microsoft.Extensions.Logging;
    using Exceptions;
    using Logging;

    public interface IProducerErrorHandler
    {
    }

    public class ProducerErrorHandler : IProducerErrorHandler
    {
        private const string FailProducerMessageLogType = "FAIL_PRODUCER_MESSAGE";

        private static ILogger<ProducerErrorHandler>? _logger;

        public ProducerErrorHandler(ILogger<ProducerErrorHandler>? logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public static void Handle(IProducer<byte[], byte[]> producer, Error error)
        {
            var exception = new ProducerErrorException(producer.Name, error.Reason);
            
            _logger?.LogError(exception, $"[{KafkaClientLogField.LogType}] - {KafkaClientLogField.ConfluentKafkaError}",
                FailProducerMessageLogType,
                error);
        }
    }
}