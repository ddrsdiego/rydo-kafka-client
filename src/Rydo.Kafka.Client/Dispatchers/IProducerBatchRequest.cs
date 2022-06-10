namespace Rydo.Kafka.Client.Dispatchers
{
    using System;
    using System.Collections.Generic;

    public interface IProducerBatchRequest : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The topic to produce the message to.
        /// </summary>
        string TopicName { get; }

        /// <summary>
        /// 
        /// </summary>
        IReadOnlyCollection<ProducerRequest> Items { get; }

        /// <summary>
        /// Adds a message (TValue) to the batch that should be sent.
        /// </summary>
        /// <param name="messageValue">The message to produce.</param>
        void Add(object messageValue);

        /// <summary>
        /// Adds a key (always is string) and message (TValue) to the batch that should be sent.
        /// </summary>
        /// <param name="messageKey">Gets the message key value (always is string).</param>
        /// <param name="messageValue">The message to produce.</param>
        void Add(string messageKey, object messageValue);
    }
}