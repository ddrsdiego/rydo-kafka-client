namespace Rydo.Kafka.Client.Serializations.Deserializers
{
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.Json;
    using Microsoft.Extensions.Logging;

    public sealed class MessageDispatcherValueUtf8BytesDeserializer : IMessageDispatcherValueDeserializer<byte[]>
    {
        private readonly ILogger<MessageDispatcherValueUtf8BytesDeserializer> _logger;

        public MessageDispatcherValueUtf8BytesDeserializer(ILogger<MessageDispatcherValueUtf8BytesDeserializer> logger)
        {
            _logger = logger;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMessage? GetMessageValue<TMessage>(byte[] request)
        {
            var message = Encoding.UTF8.GetString(request);
            
            return JsonSerializer.Deserialize<TMessage>(message, SystemTextJsonMessageSerializer.Options);
        }
    }
}