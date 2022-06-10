namespace Rydo.Kafka.Client.Serializations.Deserializers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using Microsoft.Extensions.Logging;

    internal sealed class MessageDispatcherKeyStringDeserializer : IMessageDispatcherKeyDeserializer<string>
    {
        private readonly ILogger<MessageDispatcherKeyStringDeserializer> _logger;

        public MessageDispatcherKeyStringDeserializer(ILogger<MessageDispatcherKeyStringDeserializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]        
        public TMessage? GetMessageKey<TMessage>(string request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return JsonSerializer.Deserialize<TMessage>(request, SystemTextJsonMessageSerializer.Options);
        }
    }
}