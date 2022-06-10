namespace Rydo.Kafka.Client.Producers
{
    using System;
    using System.Collections.Immutable;
    using System.Runtime.CompilerServices;
    using Configurations;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using Models;

    internal sealed class ProducerContextContainer : IProducerContextContainer<byte[], byte[]>
    {
        private readonly IServiceCollection _services;

        public ProducerContextContainer(IServiceCollection services)
        {
            _services = services;
            Entries = ImmutableDictionary<string, ProducerContext<byte[], byte[]>?>.Empty;
        }

        public ImmutableDictionary<string, ProducerContext<byte[], byte[]>?> Entries { get; private set; }

        public void AddProducer(TopicDefinition topicDefinition, Action<IProducerConfigBuilder>? specification = null)
        {
            if (string.IsNullOrEmpty(topicDefinition.TopicName))
                throw new ArgumentNullException("topicDefinition.TopicName");

            if (Entries.TryGetValue(topicDefinition.TopicName, out _))
                return;

            var producerConfig = GetProducerConfig(specification);

            var producerBuilder = new ProducerBuilder<byte[], byte[]>(producerConfig);
            var producerSpecification = new ProducerSpecification(topicDefinition.TopicName, topicDefinition.CryptKey);

            var producerContext =
                new ProducerContext<byte[], byte[]>(producerSpecification, producerConfig, producerBuilder);

            Entries = Entries.Add(topicDefinition.TopicName, producerContext);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetProducer(string topicName, out ProducerContext<byte[], byte[]>? producerContext)
        {
            if (topicName == null || string.IsNullOrEmpty(topicName))
                throw new ArgumentNullException(nameof(topicName));

            producerContext = default;

            var topic = new Topic(topicName);
            if (!Entries.TryGetValue(topic.Name, out producerContext))
                return false;

            return true;
        }

        private static ProducerConfig GetProducerConfig(Action<IProducerConfigBuilder>? specification)
        {
            IProducerConfigBuilder producerConfigBuilder = new ProducerConfigBuilder();

            if (specification == null)
                return producerConfigBuilder.GetProducerConfig();

            specification(producerConfigBuilder);

            return producerConfigBuilder.GetProducerConfig();
        }
    }
}