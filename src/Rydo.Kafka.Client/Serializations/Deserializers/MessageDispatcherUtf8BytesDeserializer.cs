namespace Rydo.Kafka.Client.Serializations.Deserializers
{
    internal sealed class MessageDispatcherUtf8BytesDeserializer : IMessageDispatcherDeserializer<string, byte[]>
    {
        public MessageDispatcherUtf8BytesDeserializer(IMessageDispatcherKeyDeserializer<string> key,
            IMessageDispatcherValueDeserializer<byte[]> value)
        {
            Key = key;
            Value = value;
        }

        public IMessageDispatcherKeyDeserializer<string> Key { get; }

        public IMessageDispatcherValueDeserializer<byte[]> Value { get; }
    }
}