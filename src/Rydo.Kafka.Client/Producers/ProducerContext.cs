namespace Rydo.Kafka.Client.Producers
{
    using System;
    using System.Text.Json;
    using Confluent.Kafka;
    using Serializations;

    public sealed class ProducerContext<TKey, TValue>
    {
        public ProducerContext(ProducerSpecification producerSpecification, ProducerConfig? producerConfig,
            ProducerBuilder<TKey, TValue> builder)
        {
            ProducerConfig = producerConfig ?? throw new ArgumentNullException(nameof(producerConfig));
            ProducerBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
            ProducerSpecification =
                producerSpecification ?? throw new ArgumentNullException(nameof(producerSpecification));
        }

        public readonly ProducerSpecification ProducerSpecification;
        public readonly ProducerConfig? ProducerConfig;
        public readonly ProducerBuilder<TKey, TValue> ProducerBuilder;

        public override string ToString() =>
            JsonSerializer.Serialize(ProducerConfig, SystemTextJsonMessageSerializer.Options);
    }
}