namespace Rydo.Kafka.Client.Serializations.Serializers
{
    using System;

    internal sealed class MessageDispatcherUtf8BytesSerializer : IMessageDispatcherSerializer<byte[], byte[]>
    {
        public MessageDispatcherUtf8BytesSerializer(IMessageDispatcherKeySerializer<byte[]> key,
            IMessageDispatcherValueSerializer<byte[]> value)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IMessageDispatcherKeySerializer<byte[]> Key { get; }

        public IMessageDispatcherValueSerializer<byte[]> Value { get; }
    }
}