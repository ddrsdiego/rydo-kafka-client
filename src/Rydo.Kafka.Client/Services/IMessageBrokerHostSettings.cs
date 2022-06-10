namespace Rydo.Kafka.Client.Services
{
    internal interface IMessageBrokerHostSettings
    {
        string Host { get; }
        string Username { get; }
        string Password { get; }
        ushort Port { get; }
    }
}