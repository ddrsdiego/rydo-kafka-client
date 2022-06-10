namespace Rydo.Kafka.Client.Serializations.Serializers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using Dispatchers;
    using Microsoft.Extensions.Logging;

    public sealed class MessageDispatcherValueStringSerializer : IMessageDispatcherValueSerializer<string>
    {
        private readonly ILogger<MessageDispatcherValueStringSerializer> _logger;

        public MessageDispatcherValueStringSerializer(ILogger<MessageDispatcherValueStringSerializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetMessageValue(ProducerRequest request)
        {
            try
            {
                var payload =
                    JsonSerializer.SerializeToUtf8Bytes(request.MessageValue, SystemTextJsonMessageSerializer.Options);
                
                var kafkaEnvelope = new KafkaEnvelope(request.Topic.Name, request.MessageKey, payload, null);
                var messageValue = JsonSerializer.Serialize(kafkaEnvelope, SystemTextJsonMessageSerializer.Options);

                return messageValue;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
                throw;
            }
        }
    }
}