namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Collections.Generic;

    public interface IConsumerRecords : IEnumerable<ConsumerRecord>
    {
        string BatchId { get; }
        
        int Count { get; }

        IEnumerable<ConsumerRecord> Faults { get; }
        
        bool HasFaults { get; }

        void Add(ConsumerRecord consumerRecord);

        /// <summary>
        /// Method that will manage ConsumerRecords that are marked as uncommitted and that must enter the retry process.
        /// </summary>
        /// <param name="consumerRecord">Current ConsumerRecord that should be marked as uncommitted and start retry process.</param>
        /// <param name="reason">Reason for failure to process the current ConsumerRecord.</param>
        /// <param name="exception"></param>
        void MarkToRetry(ConsumerRecord consumerRecord, string reason, Exception? exception);
    }
}