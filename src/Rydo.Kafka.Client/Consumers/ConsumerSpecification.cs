namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using Configurations;
    using Models;

    public class ConsumerSpecification
    {
        public ConsumerSpecification(string topicName, string groupId)
            : this(topicName, groupId, 0, string.Empty)
        {
        }

        public ConsumerSpecification(string topicName, string groupId, int retryAttempts, string retryInterval)
        {
            if (topicName == null || string.IsNullOrEmpty(topicName))
                throw new ArgumentNullException(nameof(topicName));
            
            if (groupId == null || string.IsNullOrEmpty(groupId))
                throw new ArgumentNullException(nameof(groupId));

            Topic = new Topic(topicName);
            GroupId = new GroupId(groupId);

            TimeSpan interval = default;

            if (HasDeadLetterConfigs(retryAttempts, retryInterval))
                interval = ExtractTimeSpanStrategy(retryInterval);

            DeadLetter = retryAttempts == 0
                ? null
                : new DeadLetterSpecification(retryAttempts, interval);
        }

        private static bool HasDeadLetterConfigs(int retryAttempts, string retryInterval) =>
            retryAttempts > 0 && !string.IsNullOrEmpty(retryInterval);

        private static TimeSpan ExtractTimeSpanStrategy(string retryInterval)
        {
            var length = retryInterval.Length;

            var intervalTime = Convert.ToInt32(retryInterval.Substring(0, length - 1));
            var intervalType = retryInterval[length - 1];

            var interval = intervalType switch
            {
                's' => TimeSpan.FromSeconds(intervalTime),
                'm' => TimeSpan.FromMinutes(intervalTime),
                'h' => TimeSpan.FromHours(intervalTime),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return interval;
        }

        public Topic Topic { get; }
        public GroupId GroupId { get; }
        public DeadLetterSpecification? DeadLetter { get; }
    }
}