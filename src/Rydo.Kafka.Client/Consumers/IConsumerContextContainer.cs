namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Collections.Generic;
    using Configurations;

    public interface IConsumerContextContainer<TKey, TValue>
    {
        IDictionary<string, ConsumerContext<TKey, TValue>?> Entries { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicDefinition"></param>
        /// <param name="clientSpec"></param>
        /// <param name="specification"></param>
        void AddConsumer(TopicDefinition topicDefinition, (Type? MessageContractType, Type? MessageHandlerType) clientSpec,
            Action<IConsumerConfigBuilder>? specification = null);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="consumerContext"></param>
        /// <returns></returns>
        bool TryGetConsumerContext(string topicName, out ConsumerContext<TKey, TValue>? consumerContext);
    }
}