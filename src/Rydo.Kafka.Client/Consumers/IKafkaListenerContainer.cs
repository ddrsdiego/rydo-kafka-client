namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Collections.Generic;

    public interface IKafkaListenerContainer<TKey, TValue>
    {
        IServiceProvider? Provider { get; }

        void SetServiceProvider(IServiceProvider? provider);
        
        IDictionary<string, IKafkaListener<TKey, TValue>> Listeners { get; }

        void AddListener(string topicName, IKafkaListener<TKey, TValue> listener);
    }
}