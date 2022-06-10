namespace Rydo.Kafka.Client.Sample.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Models;
    using Dispatchers;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("async")]
    public class AsyncController : ControllerBase
    {
        private readonly IMessageDispatcher _messageDispatcher;

        public AsyncController(IMessageDispatcher messageDispatcher)
        {
            _messageDispatcher = messageDispatcher;
        }

        [HttpGet("account/{accountNumber}/{key}")]
        public async Task<ActionResult> GetAccountBalance(string accountNumber, string key)
        {
            var random = new Random();

            var accountBalance = new AccountBalance
            {
                AccountNumber = accountNumber, Balance = random.Next(10, 10000),
            };
            
            var response = await _messageDispatcher.SendAsync("account-created", key, accountBalance);
            return Ok(new
            {
                AccountBalance = response.Request.MessageValue,
                response.PersistenceStatus,
                TimeElapsed = response.ElapsedTimeMs
            });
        }

        [HttpGet("/{topicName}/{accountNumber}")]
        public async Task<ActionResult> SendToTopic(string topicName, string accountNumber)
        {
            var random = new Random();

            switch (topicName)
            {
                case "account-created":
                {
                    var accountBalance = new AccountBalance
                    {
                        Balance = random.Next(10, 10000),
                        AccountNumber = accountNumber,
                    };
                    
                    var response = await _messageDispatcher.SendAsync("account-created", accountBalance.AccountNumber,
                        accountBalance);

                    return Ok(new
                    {
                        AccountBalance = response.Request.MessageValue,
                        PersistenceStatus = response.PersistenceStatus.ToString(),
                        TimeElapsed = response.ElapsedTimeMs
                    });
                }
                case "account-updated":
                {
                    var accountUpdated = new AccountUpdated
                    {
                        AccountNumber = accountNumber, Email = $"email-updated-{random.Next(1, 100)}@email.com"
                    };

                    var response = await _messageDispatcher.SendAsync("account-updated", accountUpdated);

                    return Ok(new
                    {
                        AccountUpdated = accountUpdated,
                        PersistenceStatus = response.PersistenceStatus.ToString(),
                        TimeElapsed = response.ElapsedTimeMs
                    });
                }
                default:
                    return Ok();
            }
        }

        [HttpGet("producer/balance/{amount:int}")]
        public async Task<ActionResult> GetAccountBalance(int amount)
        {
            var random = new Random();
            var batch = ProducerBatchRequest.Create("message-test");

            var keys = new LinkedList<string>();
            
            keys.AddLast("A");
            keys.AddLast("B");
            keys.AddLast("C");
            keys.AddLast("D");
            keys.AddLast("E");
            keys.AddLast("F");
            
            foreach (var key in keys)
            {
                for (var index = 0; index < amount; index++)
                {
                    var accountBalance = new AccountBalance
                    {
                        Balance = random.Next(10, 10000),
                        AccountNumber = index.ToString("00000"),
                    };

                    batch.Add(key, accountBalance);
                }
            }

            await _messageDispatcher.BatchSendAsync(batch);
            
            return Ok();
        }
    }
}