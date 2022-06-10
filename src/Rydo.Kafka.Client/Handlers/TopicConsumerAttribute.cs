namespace Rydo.Kafka.Client.Handlers
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class TopicConsumerAttribute : Attribute
    {
        private readonly string _groupId;
        private readonly string _topicName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName"></param>
        public TopicConsumerAttribute(string topicName)
            : this(topicName, string.Empty)
        {
        }

        public TopicConsumerAttribute(string topicName, string groupId)
        {
            _groupId = groupId;
            _topicName = topicName.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(topicName));
        }
    }
}