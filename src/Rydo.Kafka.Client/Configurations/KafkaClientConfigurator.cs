namespace Rydo.Kafka.Client.Configurations
{
    using System;
    using Consumers;
    using Microsoft.Extensions.DependencyInjection;
    using Producers;

    internal sealed class KafkaClientConfigurator : IKafkaClientConfigurator<byte[], byte[]>
    {
        public KafkaClientConfigurator(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            Producer = new ProducerConfigurator(services);
            Consumer = new ConsumerConfigurator(services);
        }

        public IProducerConfigurator<byte[], byte[]> Producer { get; }
        
        public IConsumerConfigurator<byte[], byte[]> Consumer { get; }
    }
}