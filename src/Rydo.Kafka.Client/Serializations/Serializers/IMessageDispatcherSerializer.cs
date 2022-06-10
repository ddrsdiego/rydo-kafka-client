namespace Rydo.Kafka.Client.Serializations.Serializers
{
    public interface IMessageDispatcherSerializer<out TKeyOut, out TValueOut>
    {
        IMessageDispatcherKeySerializer<TKeyOut> Key { get; }

        IMessageDispatcherValueSerializer<TValueOut> Value { get; }
    }
}