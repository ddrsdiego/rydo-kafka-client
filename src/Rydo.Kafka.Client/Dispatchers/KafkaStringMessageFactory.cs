namespace Rydo.Kafka.Client.Dispatchers
{
    using System;
    using Confluent.Kafka;
    using CSharpFunctionalExtensions;
    using Extensions;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Serializations.Serializers;

    public sealed class KafkaStringMessageFactory : IKafkaMessageFactory<string>
    {
        private const string ErrorCreateKafkaMessage = "ERROR-CREATE-KAFKA-MESSAGE";
        private readonly ILogger<KafkaStringMessageFactory> _logger;
        private readonly IMessageDispatcherSerializer<string, string> _messageDispatcherSerializer;

        public KafkaStringMessageFactory(ILogger<KafkaStringMessageFactory> logger,
            IMessageDispatcherSerializer<string, string> messageDispatcherSerializer)
        {
            _logger = logger;
            _messageDispatcherSerializer = messageDispatcherSerializer;
        }

        public Result<Message<string, string>> CreateKafkaMessage(ProducerRequest producerRequest)
        {
            try
            {
                var messageKey = _messageDispatcherSerializer.Key.GetMessageKey(producerRequest);
                var messageValue = _messageDispatcherSerializer.Value.GetMessageValue(producerRequest);

                var kafkaMessage = new Message<string, string>
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

                return Result.Failure<Message<string, string>>(e.Message);
            }
        }
    }
}