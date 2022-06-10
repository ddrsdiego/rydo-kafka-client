namespace Rydo.Kafka.Client.Services
{
    using System;

    internal readonly struct KafkaMessageBrokerHostSettings : IMessageBrokerHostSettings
    {
        private KafkaMessageBrokerHostSettings(string host, string username, string password, ushort port)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Port = port;
        }

        public string Host { get; }
        public string Username { get; }
        public string Password { get; }
        public ushort Port { get; }

        public static IMessageBrokerHostSettings? GetInstance(string host, string username, string password) =>
            GetInstance(host, username, password, default);

        public static IMessageBrokerHostSettings GetInstance(string host, string username, string password,
            ushort port) =>
            new KafkaMessageBrokerHostSettings(host, username, password, port);

        public override string ToString() => $"{Host}:{Port}";
    }
}