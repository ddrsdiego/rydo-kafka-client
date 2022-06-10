namespace Rydo.Kafka.Client.Services
{
    using System;
    using Constants;

    internal static class MessageBrokerHostSettingsDiscovery
    {
        public static IMessageBrokerHostSettings? GetSettings()
        {
            const string username = "test";
            const string aspnetcoreEnvironment = "ASPNETCORE_ENVIRONMENT";

            var settings = Environment.GetEnvironmentVariable(aspnetcoreEnvironment) switch
            {
                Environments.Local => KafkaMessageBrokerHostSettings.GetInstance("localhost", string.Empty,
                    string.Empty, 9092),
                Environments.Development => KafkaMessageBrokerHostSettings.GetInstance("localhost", string.Empty,
                    string.Empty, 9092),
                _ => KafkaMessageBrokerHostSettings.GetInstance("localhost", string.Empty,
                    string.Empty, 9092),
            };

            return settings;
        }
    }
}