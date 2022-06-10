namespace Rydo.Kafka.Client.Middlewares.Consumers
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Client.Consumers.Extensions;
    using Microsoft.Extensions.Logging;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Middlewares;

    public sealed class CustomConsumerMiddleware : IMessageMiddleware
    {
        private readonly IServiceProvider? _serviceProvider;
        private readonly ILogger<CustomConsumerMiddleware> _logger;

        public CustomConsumerMiddleware(ILogger<CustomConsumerMiddleware> logger, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task? Invoke(MessageConsumerContext context, MiddlewareDelegate next)
        {
            if (!context.ConsumerRecords.Any())
            {
                await next(context)!;
                return;
            }

            using (var scope = (_serviceProvider ?? throw new InvalidOperationException()).CreateScope())
            {
                if (scope.ServiceProvider.GetService(context.HandlerType ?? throw new InvalidOperationException()) is
                    IConsumerHandler messageHandler)
                {
                    var sw = Stopwatch.StartNew();

                    messageHandler.LogStartingConsumerHandler(context.HandlerType.Name, _logger);

                    try
                    {
                        await messageHandler.Consumer(context);
                    }
                    catch (Exception e)
                    {
                        foreach (var consumerRecord in context.ConsumerRecords)
                        {
                            context.ConsumerRecords.MarkToRetry(consumerRecord, "", e);
                        }
                    }
                    finally
                    {
                        sw.Stop();
                        messageHandler.LogFinishingConsumerHandler(context.HandlerType.Name, _logger, sw);
                    }
                }
            }

            await next(context)!;
        }
    }
}