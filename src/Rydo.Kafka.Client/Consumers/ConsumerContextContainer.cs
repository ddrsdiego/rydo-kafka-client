namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Configurations;
    using Confluent.Kafka;
    using Models;

    internal sealed class ConsumerContextContainer : IConsumerContextContainer<byte[], byte[]>
    {
        private ImmutableDictionary<string, ConsumerContext<byte[], byte[]>?> _entries;

        internal ConsumerContextContainer()
        {
            _entries = ImmutableDictionary<string, ConsumerContext<byte[], byte[]>?>.Empty;
        }

        public IDictionary<string, ConsumerContext<byte[], byte[]>?> Entries => _entries;

        public void AddConsumer(TopicDefinition topicDefinition,
            (Type? MessageContractType, Type? MessageHandlerType) clientSpec,
            Action<IConsumerConfigBuilder>? specification = null)
        {
            if (topicDefinition == null || string.IsNullOrEmpty(topicDefinition.TopicName))
                throw new ArgumentNullException(nameof(topicDefinition));

            if (_entries.TryGetValue(topicDefinition.TopicName, out _))
                throw new InvalidOperationException("topicDefinition.TopicName");

            var consumerSpecification = new ConsumerSpecification(topicDefinition.TopicName,
                topicDefinition.ConsumerGroup, topicDefinition.RetryAttempts, topicDefinition.RetryInterval);

            var consumerConfig = GetConsumerConfig(consumerSpecification, specification);

            var consumerBuilder = new ConsumerBuilder<byte[], byte[]>(consumerConfig);

            var consumerContext =
                new ConsumerContext<byte[], byte[]>(consumerSpecification, consumerConfig, consumerBuilder,
                    clientSpec.MessageContractType, clientSpec.MessageHandlerType);

            _entries = _entries.Add(consumerSpecification.Topic.Name, consumerContext);
        }

        private static ConsumerConfig? GetConsumerConfig(ConsumerSpecification consumerSpecification,
            Action<IConsumerConfigBuilder>? specification)
        {
            IConsumerConfigBuilder consumerConfigBuilder = new ConsumerConfigBuilder();

            specification?.Invoke(consumerConfigBuilder);

            var consumerConfig = consumerConfigBuilder
                .GroupId(consumerSpecification.GroupId.Value)
                .GetConsumerConfig();

            return consumerConfig;
        }

        public bool TryGetConsumerContext(string topicName, out ConsumerContext<byte[], byte[]>? consumerContext)
        {
            consumerContext = default;
            return _entries.TryGetValue(new Topic(topicName).Name, out consumerContext);
        }
    }
}