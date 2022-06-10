namespace Rydo.Kafka.Client.Serializations.Deserializers
{
    internal sealed class MessageDispatcherStringDeserializer : IMessageDispatcherDeserializer<string, string>
    {
        public MessageDispatcherStringDeserializer(IMessageDispatcherKeyDeserializer<string> key,
            IMessageDispatcherValueDeserializer<string> value)
        {
            Key = key;
            Value = value;
        }

        public IMessageDispatcherKeyDeserializer<string> Key { get; }

        public IMessageDispatcherValueDeserializer<string> Value { get; }
    }
}