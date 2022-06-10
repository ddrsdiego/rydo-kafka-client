namespace Rydo.Kafka.Client.Sample.Core.Consumers
{
    using System;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.Logging;
    using Models;
    using Repositories;

    [TopicConsumer("ACCOUNT-CREATED")]
    public class AccountCreatedConsumerHandler : ConsumerHandler<AccountBalance>
    {
        private readonly ILogger<AccountCreatedConsumerHandler> _logger;
        private readonly IAccountRepository _accountRepository;

        public AccountCreatedConsumerHandler(ILogger<AccountCreatedConsumerHandler> logger,
            IAccountRepository accountRepository)
        {
            _logger = logger;
            _accountRepository = accountRepository;
        }

        public override async Task Consumer(MessageConsumerContext context)
        {
            const string topicName = "account-updated";

            _logger.LogInformation($"Receving message in {context.Received.ConsumerName}");

            foreach (var consumerRecord in context.ConsumerRecords)
            {
                var accountBalance = consumerRecord.Value<AccountBalance>();
                if (accountBalance is null)
                    return;
                
                await _accountRepository.RegisterBalance(accountBalance!);

                var accountUpdated = new AccountUpdated
                {
                    AccountNumber = accountBalance.AccountNumber,
                    Email = $"email-user-{Guid.NewGuid().ToString().Split('-')[0]}@example.com"
                };

                // await context.Dispatcher.SendAsync(topicName, accountBalance.AccountNumber, accountUpdated);
                //context.ConsumerRecords.MarkToRetry(consumerRecord, "reason", new Exception());
            }
        }
    }
}