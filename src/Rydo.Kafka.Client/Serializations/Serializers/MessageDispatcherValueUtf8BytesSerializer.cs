namespace Rydo.Kafka.Client.Serializations.Serializers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.Json;
    using Microsoft.Extensions.Logging;
    using Dispatchers;

    public sealed class MessageDispatcherValueUtf8BytesSerializer : IMessageDispatcherValueSerializer<byte[]>
    {
        private readonly ILogger<MessageDispatcherValueUtf8BytesSerializer> _logger;

        public MessageDispatcherValueUtf8BytesSerializer(ILogger<MessageDispatcherValueUtf8BytesSerializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetMessageValue(ProducerRequest request)
        {
            try
            {
                var payload =
                    JsonSerializer.SerializeToUtf8Bytes(request.MessageValue, SystemTextJsonMessageSerializer.Options);
                
                var kafkaEnvelope = new KafkaEnvelope(request.Topic.Name, request.MessageKey, payload, null);
                
                return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(kafkaEnvelope, SystemTextJsonMessageSerializer.Options));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
                throw;
            }
        }
    }
}