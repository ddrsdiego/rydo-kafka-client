namespace Rydo.Kafka.Client.Producers
{
    using System;
    using System.Collections.Immutable;
    using Configurations;

    public interface IProducerContextContainer<TKey, TValue>
    {
        void AddProducer(TopicDefinition topicDefinition, Action<IProducerConfigBuilder>? specification = null);

        ImmutableDictionary<string, ProducerContext<byte[], byte[]>?> Entries { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="producerContext"></param>
        /// <returns></returns>
        bool TryGetProducer(string topicName, out ProducerContext<TKey, TValue>? producerContext);
    }
}