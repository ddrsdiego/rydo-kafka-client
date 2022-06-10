namespace Rydo.Kafka.Client.Middlewares.Consumers
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Constants;
    using Dispatchers;
    using Microsoft.Extensions.Logging;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Middlewares;
    using Serializations;

    public sealed class DeadLetterHandleMiddleware : IMessageMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DeadLetterHandleMiddleware> _logger;

        public DeadLetterHandleMiddleware(ILogger<DeadLetterHandleMiddleware> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task? Invoke(MessageConsumerContext context, MiddlewareDelegate next)
        {
            if (!HasDeadLetterForProcess(context))
            {
                await next(context)!;
                return;
            }

            try
            {
                var consumerRecord = context.ConsumerRecords.Faults.First();

                var jsonAsString = consumerRecord.ValueAsJsonString();
                var contractType = context.HandlerType;
                var value = JsonSerializer.Deserialize(jsonAsString!, contractType!,
                    SystemTextJsonMessageSerializer.Options);

                var topicDeadLetter = TopicConventions.GetTopicDeadLetter(consumerRecord.TopicName);
                var producerBatchRequest = ProducerBatchRequest.Create(topicDeadLetter);

                foreach (var consumerRecordFault in context.ConsumerRecords.Faults)
                {
                    producerBatchRequest.Add(consumerRecordFault.Key(), value);
                }

                using var scope = _serviceProvider.CreateScope();
                
                await scope.ServiceProvider.GetRequiredService<IMessageDispatcher>()
                    .BatchSendAsync(producerBatchRequest, CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }

            await next(context)!;
        }

        private static bool HasDeadLetterForProcess(MessageConsumerContext messageConsumerContext) =>
            messageConsumerContext.ConsumerRecords.HasFaults &&
            messageConsumerContext.ConsumerRecords.Faults.Any(consumerRecord => consumerRecord.DeadLetterSpec != null);
    }
}