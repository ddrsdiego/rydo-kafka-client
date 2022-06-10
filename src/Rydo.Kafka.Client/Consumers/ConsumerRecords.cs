namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    public sealed class ConsumerRecords : IConsumerRecords
    {
        private readonly object _syncLock;
        private readonly LinkedList<ConsumerRecord> _consumerRecords;
        private LinkedList<ConsumerRecord>? _consumerRecordsToRetry;

        internal ConsumerRecords()
        {
            _syncLock = new object();

            HasFaults = false;

            BatchId = Guid.NewGuid().ToString().Split('-')[0];
            _consumerRecords = new LinkedList<ConsumerRecord>();
        }

        public string BatchId { get; }

        public int Count
        {
            get
            {
                lock (_syncLock)
                {
                    return _consumerRecords.Count;
                }
            }
        }

        public bool HasFaults { get; private set; }

        public void Add(ConsumerRecord consumerRecord) => InternalAdd(consumerRecord);

        public IEnumerator<ConsumerRecord> GetEnumerator()
        {
            foreach (var consumerRecord in _consumerRecords)
            {
                yield return consumerRecord;
            }
        }

        public IEnumerable<ConsumerRecord> Faults
        {
            get
            {
                lock (_syncLock)
                {
                    Debug.Assert(_consumerRecordsToRetry != null, nameof(_consumerRecordsToRetry) + " != null");
                    
                    foreach (var consumerRecord in _consumerRecordsToRetry)
                        yield return consumerRecord;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void InternalAdd(ConsumerRecord consumerRecord)
        {
            lock (_syncLock)
            {
                _consumerRecords.AddLast(consumerRecord);
            }
        }

        public void MarkToRetry(ConsumerRecord consumerRecord, string reason, Exception? exception = null)
        {
            lock (_syncLock)
            {
                _consumerRecordsToRetry ??= new LinkedList<ConsumerRecord>();
                HasFaults = true;
                _consumerRecordsToRetry.AddLast(consumerRecord);
            }
        }
    }
}