namespace Rydo.Kafka.Client.Sample.Core.Consumers
{
    using System;
    using System.Threading.Tasks;
    using Dispatchers;
    using Handlers;
    using Microsoft.Extensions.Logging;
    using Models;

    [TopicConsumer("ACCOUNT-UPDATED")]
    public class AccountUpdatedConsumerHandler : ConsumerHandler<AccountUpdated>
    {
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly ILogger<AccountUpdatedConsumerHandler> _logger;

        public AccountUpdatedConsumerHandler(IMessageDispatcher messageDispatcher,
            ILogger<AccountUpdatedConsumerHandler> logger)
        {
            _logger = logger;
            _messageDispatcher = messageDispatcher;
        }

        public override async Task Consumer(MessageConsumerContext context)
        {
            var random = new Random();
            
            _logger.LogInformation($"Receving message in {context.Received.ConsumerName}");
            
            foreach (var consumerRecord in context.ConsumerRecords)
            {
                try
                {
                    var accountUpdated = consumerRecord.Value<AccountUpdated>();

                    await Task.Delay(250, context.CancellationToken);

                    var accountBalance = new AccountBalance
                    {
                        Balance = random.Next(10, 10000),
                        AccountNumber = accountUpdated?.AccountNumber
                    };

                    _logger.LogInformation(
                        $"Message {consumerRecord.Key()} with body: {consumerRecord.ValueAsJsonString()} - {DateTime.Now}");
                }
                catch (Exception e)
                {
                    context.ConsumerRecords.MarkToRetry(consumerRecord,"test", new Exception());
                }
            }
        }
    }
}