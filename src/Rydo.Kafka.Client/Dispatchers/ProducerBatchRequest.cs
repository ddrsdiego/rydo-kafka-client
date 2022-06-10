namespace Rydo.Kafka.Client.Dispatchers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    public readonly struct ProducerBatchRequest : IProducerBatchRequest
    {
        private readonly object _syncObject;
        private readonly LinkedList<ProducerRequest> _items;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName">The topic to produce the message to.</param>
        /// <exception cref="ArgumentNullException"></exception>
        private ProducerBatchRequest(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
                throw new ArgumentNullException(nameof(topicName));

            _syncObject = new object();

            TopicName = topicName.ToUpperInvariant();

            _items = new LinkedList<ProducerRequest>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName">The topic the record will be appended to.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static IProducerBatchRequest Create(string topicName) => new ProducerBatchRequest(topicName);

        public int Count => _items.Count;

        public string TopicName { get; }

        public IReadOnlyCollection<ProducerRequest> Items => _items.ToImmutableList();

        public void Add(object messageValue)
        {
            if (messageValue == null) throw new ArgumentNullException(nameof(messageValue));

            InternalAdd(ProducerRequest.Create(TopicName, messageValue));
        }

        public void Add(string messageKey, object messageValue)
        {
            if (messageKey == null) throw new ArgumentNullException(nameof(messageKey));

            if (messageValue == null) throw new ArgumentNullException(nameof(messageValue));

            InternalAdd(ProducerRequest.Create(TopicName, messageKey, messageValue));
        }

        private void InternalAdd(ProducerRequest item)
        {
            lock (_syncObject)
            {
                _items.AddLast(item);
            }
        }

        public void Dispose() => _items.Clear();
    }
}