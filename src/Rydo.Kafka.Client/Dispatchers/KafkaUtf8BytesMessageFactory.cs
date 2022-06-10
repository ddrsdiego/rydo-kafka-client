namespace Rydo.Kafka.Client.Dispatchers
{
    using System;
    using Confluent.Kafka;
    using CSharpFunctionalExtensions;
    using Extensions;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Serializations.Serializers;

    public sealed class KafkaUtf8BytesMessageFactory : IKafkaMessageFactory<byte[]>
    {
        private const string ErrorCreateKafkaMessage = "ERROR-CREATE-KAFKA-MESSAGE";
        private readonly ILogger<KafkaUtf8BytesMessageFactory> _logger;
        private readonly IMessageDispatcherSerializer<byte[], byte[]> _messageDispatcherSerializer;

        public KafkaUtf8BytesMessageFactory(ILogger<KafkaUtf8BytesMessageFactory> logger,
            IMessageDispatcherSerializer<byte[], byte[]> messageDispatcherSerializer)
        {
            _logger = logger;
            _messageDispatcherSerializer = messageDispatcherSerializer;
        }

        public Result<Message<byte[], byte[]>> CreateKafkaMessage(ProducerRequest producerRequest)
        {
            try
            {
                var messageKey = _messageDispatcherSerializer.Key.GetMessageKey(producerRequest);
                var messageValue = _messageDispatcherSerializer.Value.GetMessageValue(producerRequest);

                var kafkaMessage = new Message<byte[], byte[]>
                {
                    Key = messageKey,
                    Value = messageValue,
                    Headers = new Headers()
                };

                kafkaMessage.FillDefaultHeaders(producerRequest, _logger);

                return Result.Success(kafkaMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"[{KafkaClientLogField.LogType}] - {KafkaClientLogField.MessageKey}",
                    ErrorCreateKafkaMessage,
                    producerRequest.MessageKey);

                return Result.Failure<Message<byte[], byte[]>>(e.Message);
            }
        }
    }
}