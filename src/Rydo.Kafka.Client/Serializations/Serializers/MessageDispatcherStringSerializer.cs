namespace Rydo.Kafka.Client.Serializations.Serializers
{
    internal sealed class MessageDispatcherStringSerializer : IMessageDispatcherSerializer<string, string>
    {
        public MessageDispatcherStringSerializer(IMessageDispatcherKeySerializer<string> key,
            IMessageDispatcherValueSerializer<string> value)
        {
            Key = key;
            Value = value;
        }

        public IMessageDispatcherKeySerializer<string> Key { get; }
        public IMessageDispatcherValueSerializer<string> Value { get; }
    }
}