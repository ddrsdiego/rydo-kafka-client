namespace Rydo.Kafka.Client.Serializations.Deserializers
{
    public interface IMessageDispatcherValueDeserializer<in TIn>
    {
        TMessage? GetMessageValue<TMessage>(TIn request);
    }
}