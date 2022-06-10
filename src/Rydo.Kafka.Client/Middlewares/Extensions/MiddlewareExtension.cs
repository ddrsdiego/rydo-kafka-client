namespace Rydo.Kafka.Client.Middlewares.Extensions
{
    using System.Collections.Generic;
    using Consumers;
    using Microsoft.Extensions.DependencyInjection;

    public static class MiddlewareExtension
    {
        public static void AddMiddlewares(this IServiceCollection services)
        {
            var configs = new List<MiddlewareConfiguration>();

            configs.Insert(0, new MiddlewareConfiguration(typeof(CustomConsumerMiddleware), ServiceLifetime.Scoped));
            
            configs.Insert(configs.Count,
                new MiddlewareConfiguration(typeof(DeadLetterHandleMiddleware), ServiceLifetime.Scoped));
            
            configs.Insert(configs.Count,
                new MiddlewareConfiguration(typeof(OffsetCommitManagerMiddleware), ServiceLifetime.Scoped));

            foreach (var middlewareConfiguration in configs)
            {
                services.Add(new ServiceDescriptor(middlewareConfiguration.Type, middlewareConfiguration.Type,
                    middlewareConfiguration.Lifetime));
            }

            services.AddSingleton(configs);
        }
    }
}