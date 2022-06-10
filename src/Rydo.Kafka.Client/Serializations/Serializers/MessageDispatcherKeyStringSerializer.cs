namespace Rydo.Kafka.Client.Serializations.Serializers
{
    using Microsoft.Extensions.Logging;
    using Dispatchers;

    public sealed class MessageDispatcherKeyStringSerializer : IMessageDispatcherKeySerializer<string>
    {
        private readonly ILogger<MessageDispatcherKeyStringSerializer> _logger;

        public MessageDispatcherKeyStringSerializer(ILogger<MessageDispatcherKeyStringSerializer> logger)
        {
            _logger = logger;
        }

        public string GetMessageKey(ProducerRequest request) => request.MessageKey;
    }
}