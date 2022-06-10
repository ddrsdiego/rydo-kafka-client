namespace Rydo.Kafka.Client.Producers
{
    using Confluent.Kafka;
    using Services;

    internal sealed class ProducerConfigBuilder : IProducerConfigBuilder
    {
        public const int RetryBackoffMsDefault = 20;
        public const Acks AcksDefault = Confluent.Kafka.Acks.All;
        public const CompressionType CompressionTypeDefault = CompressionType.Snappy;
        public const int FetchMaxBytesDefault = 1024 * 1024 * 8;
        
        private Acks _acks;

        public ProducerConfigBuilder()
        {
            _acks = AcksDefault;
        }

        public IProducerConfigBuilder Acks(Acks acks = AcksDefault)
        {
            _acks = acks;
            return this;
        }

        public ProducerConfig GetProducerConfig()
        {
            var brokerHostSettings = MessageBrokerHostSettingsDiscovery.GetSettings();
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = brokerHostSettings?.ToString(),
                Acks = _acks,
                RetryBackoffMs = RetryBackoffMsDefault,
                CompressionType = CompressionTypeDefault,
                EnableIdempotence = true,
                MessageSendMaxRetries = 3
            };

            return producerConfig;
        }
    }
}