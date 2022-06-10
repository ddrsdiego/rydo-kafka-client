namespace Rydo.Kafka.Client.Configurations
{
    using Consumers;
    using Producers;

    public interface IKafkaClientConfigurator<TKey, TValue>
    {
        IProducerConfigurator<TKey, TValue> Producer { get; }

        IConsumerConfigurator<TKey, TValue> Consumer { get; }
    }
}