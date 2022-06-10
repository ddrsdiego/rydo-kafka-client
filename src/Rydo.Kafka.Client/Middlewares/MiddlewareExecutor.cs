namespace Rydo.Kafka.Client.Middlewares
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal interface IMiddlewareExecutor
    {
        Task? Execute(IServiceScope? scope, MessageConsumerContext context,
            Func<MessageConsumerContext, Task> nextOperation);
    }

    internal class MiddlewareExecutor : IMiddlewareExecutor
    {
        private readonly ILogger<MiddlewareExecutor> _logger;
        private readonly List<MiddlewareConfiguration> _configurations;
        private readonly Dictionary<int, IMessageMiddleware?> _consumerMiddlewares;

        public MiddlewareExecutor(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<MiddlewareExecutor>>();
            _configurations = serviceProvider.GetRequiredService<List<MiddlewareConfiguration>>();
            _consumerMiddlewares = new Dictionary<int, IMessageMiddleware?>();
        }

        public Task? Execute(IServiceScope? scope, MessageConsumerContext context,
            Func<MessageConsumerContext, Task> nextOperation)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return ExecuteDefinition(0, scope, context, nextOperation);
        }

        private Task? ExecuteDefinition(int index, IServiceScope? scope, MessageConsumerContext context,
            Func<MessageConsumerContext, Task> nextOperation)
        {
            if (_configurations.Count == index)
                return nextOperation(context);

            var configuration = _configurations[index];

            var messageMiddleware = ResolveInstance(index, scope, configuration);

            return messageMiddleware!.Invoke(context,
                nextContext => ExecuteDefinition(index + 1, scope, nextContext, nextOperation));
        }

        private IMessageMiddleware? ResolveInstance(int index, IServiceScope? scope,
            MiddlewareConfiguration configuration)
        {
            IMessageMiddleware? messageMiddleware = default;

            try
            {
                messageMiddleware = _consumerMiddlewares.SafeGetOrAdd(index,
                    _ => (IMessageMiddleware) scope?.ServiceProvider.GetService(configuration.Type)!);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }

            return messageMiddleware;
        }
    }
}