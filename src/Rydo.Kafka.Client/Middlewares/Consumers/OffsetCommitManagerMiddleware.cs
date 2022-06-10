namespace Rydo.Kafka.Client.Middlewares.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Client.Consumers;
    using Client.Consumers.Extensions;
    using Handlers;
    using Logging;
    using Microsoft.Extensions.Logging;

    public sealed class OffsetCommitManagerMiddleware : IMessageMiddleware
    {
        private const string ErrorCommitConsumerRecord = "ERROR_COMMIT_CONSUMER_RECORD";

        private readonly ILogger<OffsetCommitManagerMiddleware> _logger;
        private readonly Dictionary<int, ConsumerRecord>? _consumerRecordsToCommit;

        public OffsetCommitManagerMiddleware(ILogger<OffsetCommitManagerMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _consumerRecordsToCommit = new Dictionary<int, ConsumerRecord>();
        }

        public async Task? Invoke(MessageConsumerContext context, MiddlewareDelegate next)
        {
            if (!context.ConsumerRecords.Any())
            {
                await next(context)!;
                return;
            }

            foreach (var consumerRecord in context.ConsumerRecords)
            {
                if (!_consumerRecordsToCommit!.TryGetValue(consumerRecord.Partition, out var currentConsumerRecord))
                    _consumerRecordsToCommit.Add(consumerRecord.Partition, consumerRecord);
                else
                {
                    if (consumerRecord.Offset <= currentConsumerRecord.Offset)
                        continue;

                    _consumerRecordsToCommit.Remove(consumerRecord.Partition);
                    _consumerRecordsToCommit.Add(consumerRecord.Partition, consumerRecord);
                }
            }

            if (_consumerRecordsToCommit?.Count <= 0)
            {
                await next(context)!;
                return;
            }

            foreach (var (_, consumerRecord) in _consumerRecordsToCommit!)
            {
                try
                {
                    if (context.Consumer == null)
                        continue;
                    
                    await context.Consumer.Commit(consumerRecord);
                    consumerRecord.LogMessageCommit(context.ConsumerRecords.BatchId, _logger);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"{KafkaClientLogField.LogType}",
                        ErrorCommitConsumerRecord);
                }
            }

            await next(context)!;
        }
    }
}