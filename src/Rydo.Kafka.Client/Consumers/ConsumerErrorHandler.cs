namespace Rydo.Kafka.Client.Consumers
{
    using Confluent.Kafka;
    using Exceptions;
    using Logging;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public interface IConsumerErrorHandler
    {
        public void Handle(IConsumer<byte[], byte[]> consumer, Error error);
    }

    internal sealed class ConsumerErrorHandler : IConsumerErrorHandler
    {
        private const string FailConsumerMessageLogType = "FAIL_CONSUMER_MESSAGE";

        private readonly ILogger<ConsumerErrorHandler> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public ConsumerErrorHandler(ILogger<ConsumerErrorHandler> logger,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public void Handle(IConsumer<byte[], byte[]> consumer, Error error)
        {
            var exception = new ConsumerErrorException(consumer.Name, error.Reason);

            _logger.LogError(exception, $"[{KafkaClientLogField.LogType}] - {KafkaClientLogField.ConfluentKafkaError}",
                FailConsumerMessageLogType,
                error);

            // _hostApplicationLifetime.StopApplication();
        }
    }
}