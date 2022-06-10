namespace Rydo.Kafka.Client.Sample.Core.Consumers
{
    using System;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.Logging;
    using Models;

    [TopicConsumer("MESSAGE-TEST")]
    public class MessageHandlerConsumerHandler : ConsumerHandler<AccountBalance>
    {
        private readonly ILogger<MessageHandlerConsumerHandler> _logger;

        public MessageHandlerConsumerHandler(ILogger<MessageHandlerConsumerHandler> logger)
        {
            _logger = logger;
        }

        public override Task Consumer(MessageConsumerContext context)
        {
            _logger.LogInformation($"Tamanho do lote: {context.ConsumerRecords.Count} - {DateTime.Now}");

            foreach (var consumerRecord in context.ConsumerRecords)
            {
                var accountBalance = consumerRecord.Value<AccountBalance>();
                _logger.LogInformation(
                    $"Key {consumerRecord.Key()} Acc: {accountBalance} - Par: {consumerRecord.Partition} - Offset: {consumerRecord.Offset} - {DateTime.Now}");
            }

            return Task.CompletedTask;
        }
    }
}