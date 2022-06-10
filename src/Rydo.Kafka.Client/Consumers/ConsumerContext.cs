namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Serializations;

    public sealed class ConsumerContext<TKey, TValue>
    {
        public ConsumerContext(ConsumerSpecification consumerSpecification, ConsumerConfig? consumerConfig,
            ConsumerBuilder<TKey, TValue> consumerBuilder, Type? contractType, Type? handlerType)
        {
            ConsumerSpecification = consumerSpecification;
            HandlerType = handlerType;
            ContractType = contractType;
            ConsumerConfig = consumerConfig;
            ConsumerBuilder = consumerBuilder;
        }
        
        public readonly Type? HandlerType;
        public readonly Type? ContractType;
        public readonly ConsumerConfig? ConsumerConfig;
        public readonly ConsumerSpecification ConsumerSpecification;
        public readonly ConsumerBuilder<TKey, TValue> ConsumerBuilder;

        public override string ToString() =>
            JsonSerializer.Serialize(ConsumerConfig, SystemTextJsonMessageSerializer.Options);
    }
}