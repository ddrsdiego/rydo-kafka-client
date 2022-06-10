namespace Rydo.Kafka.Client.Serializations.Deserializers
{
    public interface IMessageDispatcherKeyDeserializer<in TIn>
    {
        TMessage? GetMessageKey<TMessage>(TIn request);
    }
}