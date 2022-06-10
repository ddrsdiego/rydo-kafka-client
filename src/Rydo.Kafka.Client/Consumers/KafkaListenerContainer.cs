namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    public class KafkaListenerContainer : IKafkaListenerContainer<byte[], byte[]>
    {
        private ImmutableDictionary<string, IKafkaListener<byte[], byte[]>> _entries;

        public KafkaListenerContainer()
        {
            _entries = ImmutableDictionary<string, IKafkaListener<byte[], byte[]>>.Empty;
        }

        public IServiceProvider? Provider { get; private set; }

        public IDictionary<string, IKafkaListener<byte[], byte[]>> Listeners => _entries;

        public void AddListener(string topicName, IKafkaListener<byte[], byte[]> listener)
        {
            if (topicName == null || string.IsNullOrEmpty(topicName))
                throw new ArgumentNullException(nameof(topicName));

            if (_entries.TryGetValue(topicName, out _))
                throw new InvalidOperationException(nameof(topicName));

            _entries = _entries.Add(topicName, listener);
        }

        public void SetServiceProvider(IServiceProvider? provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));

            foreach (var kafkaListener in _entries.Values)
            {
                kafkaListener.SetupDependencies(Provider);
            }
        }
    }
}