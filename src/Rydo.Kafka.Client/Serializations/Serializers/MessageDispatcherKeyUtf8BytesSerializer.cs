namespace Rydo.Kafka.Client.Serializations.Serializers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Dispatchers;
    using Microsoft.Extensions.Logging;

    public sealed class MessageDispatcherKeyUtf8BytesSerializer : IMessageDispatcherKeySerializer<byte[]>
    {
        private readonly ILogger<MessageDispatcherKeyUtf8BytesSerializer> _logger;

        public MessageDispatcherKeyUtf8BytesSerializer(ILogger<MessageDispatcherKeyUtf8BytesSerializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetMessageKey(ProducerRequest request)
        {
            try
            {
                return Encoding.UTF8.GetBytes(request.MessageKey);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}