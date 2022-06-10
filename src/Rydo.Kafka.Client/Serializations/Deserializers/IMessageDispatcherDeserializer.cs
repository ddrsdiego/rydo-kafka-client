namespace Rydo.Kafka.Client.Serializations.Deserializers
{
    public interface IMessageDispatcherDeserializer<in TKeyIn, in TValueIn>
    {
        IMessageDispatcherKeyDeserializer<TKeyIn> Key { get; }

        IMessageDispatcherValueDeserializer<TValueIn> Value { get; }
    }
}