namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using Confluent.Kafka;
    using Services;

    internal sealed class ConsumerConfigBuilder : IConsumerConfigBuilder
    {
        private const int FetchMaxBytesDefault = 1024 * 1024 * 32;
        private const int SessionTimeoutMsDefault = 15_000;
        private const int HeartbeatIntervalMsDefault = SessionTimeoutMsDefault / 3;

        private string _groupId;
        private string _clientId;
        private int _sessionTimeoutMs;
        private int _heartbeatIntervalMs;

        public ConsumerConfigBuilder()
        {
            _groupId = string.Empty;
            _clientId = string.Empty;
            _sessionTimeoutMs = SessionTimeoutMsDefault;
            _heartbeatIntervalMs = HeartbeatIntervalMsDefault;
        }

        public IConsumerConfigBuilder SessionTimeoutMs(int sessionTimeoutMs = SessionTimeoutMsDefault)
        {
            _sessionTimeoutMs = sessionTimeoutMs;
            return this;
        }

        public IConsumerConfigBuilder HeartbeatIntervalMs(int heartbeatIntervalMs = HeartbeatIntervalMsDefault)
        {
            _heartbeatIntervalMs = heartbeatIntervalMs;
            return this;
        }

        public IConsumerConfigBuilder GroupId(string groupId)
        {
            var randomClientIdNumber = Guid.NewGuid().ToString().Split('-')[0];

            _groupId = groupId;
            _clientId = $"{groupId}-{randomClientIdNumber}";

            return this;
        }

        public ConsumerConfig GetConsumerConfig()
        {
            var brokerHostSettings = MessageBrokerHostSettingsDiscovery.GetSettings();

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = brokerHostSettings?.ToString(),
                GroupId = _groupId,
                ClientId = _clientId,
                EnableAutoCommit = false,
                SessionTimeoutMs = _sessionTimeoutMs,
                HeartbeatIntervalMs = _heartbeatIntervalMs,
                FetchMaxBytes = FetchMaxBytesDefault
            };

            return consumerConfig;
        }
    }
}