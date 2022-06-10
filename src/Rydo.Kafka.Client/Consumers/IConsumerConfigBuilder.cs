namespace Rydo.Kafka.Client.Consumers
{
    using Confluent.Kafka;

    public interface IConsumerConfigBuilder
    {
        /// <summary>
        /// The amount of time a consumer can be out of contact with the brokers while still considered alive
        /// Default is 45 seconds
        /// </summary>
        /// <param name="sessionTimeoutMs"></param>
        /// <returns></returns>
        IConsumerConfigBuilder SessionTimeoutMs(int sessionTimeoutMs);
        
        /// <summary>
        /// Sets the control on how often the Kafka consumer will send a heartbeat to the group coordinator
        /// </summary>
        /// <param name="heartbeatIntervalMs"></param>
        /// <returns></returns>
        IConsumerConfigBuilder HeartbeatIntervalMs(int heartbeatIntervalMs);
        
        /// <summary>
        /// Client group id string. All clients sharing the same group.id belong to the same group.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        IConsumerConfigBuilder GroupId(string groupId);
        
        ConsumerConfig? GetConsumerConfig();
    }
}