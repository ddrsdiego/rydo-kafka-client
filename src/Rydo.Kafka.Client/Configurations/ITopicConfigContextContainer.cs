namespace Rydo.Kafka.Client.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    public interface ITopicConfigContextContainer
    {
        ImmutableDictionary<string, TopicDefinition> Entries { get; }

        void AddTopicConfig(IEnumerable<TopicConfig> topicConfigs, DeadLetterPolicyEntry deadLetterPolicyEntry,
            Type? type);
    }
}