namespace Rydo.Kafka.Client.Dispatchers
{
    using System;
    using Models;

    public readonly struct ProducerRequest
    {
        private ProducerRequest(string key, object value, string topicName)
        {
            if (string.IsNullOrEmpty(topicName)) throw new ArgumentNullException(nameof(topicName));

            MessageKey = key ?? throw new ArgumentNullException(nameof(key));
            MessageValue = value ?? throw new ArgumentNullException(nameof(value));
            CreatedAt = DateTime.Now;
            Topic = new Topic(topicName);
        }

        public static ProducerRequest Default => new ProducerRequest();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="messageValue"></param>
        /// <returns></returns>
        public static ProducerRequest Create(string topicName, object messageValue) =>
            new ProducerRequest(Guid.NewGuid().ToString(), messageValue, topicName);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="messageKey"></param>
        /// <param name="messageValue"></param>
        /// <returns></returns>
        public static ProducerRequest Create(string topicName, string messageKey, object messageValue) =>
            new ProducerRequest(messageKey, messageValue, topicName);

        public readonly Topic Topic;
        public readonly string MessageKey;
        public readonly object MessageValue;
        public readonly DateTime CreatedAt;
    }
}