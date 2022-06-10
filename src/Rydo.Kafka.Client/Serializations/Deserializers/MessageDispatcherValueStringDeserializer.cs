namespace Rydo.Kafka.Client.Serializations.Deserializers
{
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using Microsoft.Extensions.Logging;

    public sealed class MessageDispatcherValueStringDeserializer : IMessageDispatcherValueDeserializer<string>
    {
        private readonly ILogger<MessageDispatcherValueStringDeserializer> _logger;

        public MessageDispatcherValueStringDeserializer(ILogger<MessageDispatcherValueStringDeserializer> logger)
        {
            _logger = logger;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMessage? GetMessageValue<TMessage>(string request)
        {
            return JsonSerializer.Deserialize<TMessage>(request, SystemTextJsonMessageSerializer.Options);
        }
    }
}