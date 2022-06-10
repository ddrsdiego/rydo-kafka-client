namespace Rydo.Kafka.Client.UnitTest.Middlewares
{
    using System;
    using Client.Middlewares;
    using Microsoft.Extensions.DependencyInjection;

    public static class MiddlewaresEx
    {
        public static void AddMiddleware<T>(this IServiceCollection services, ServiceLifetime serviceLifetime)
            where T : IMessageMiddleware
        {
            var configuration = new MiddlewareConfiguration(typeof(T), serviceLifetime);

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.Add(
                        new ServiceDescriptor(typeof(IMessageMiddleware), typeof(T), ServiceLifetime.Singleton));

                    // services.AddSingleton(typeof(IMessageMiddleware));
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(typeof(IMessageMiddleware));
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(typeof(IMessageMiddleware));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
            }
        }
    }
    //
    // public class MiddlewaresTest
    // {
    //
    //     
    //     [Fact]
    //     public async Task Test()
    //     {
    //         var serviceCollection = new ServiceCollection();
    //
    //         var configs = new List<MiddlewareConfiguration>
    //         {
    //             new MiddlewareConfiguration(typeof(CustomConsumerHandler), ServiceLifetime.Scoped),
    //             new MiddlewareConfiguration(typeof(CommitRecordsHandler), ServiceLifetime.Scoped)
    //         };
    //         var microsoftDependencyConfigurator = new MicrosoftDependencyConfigurator(serviceCollection);
    //
    //         var middlewareExecutor = new MiddlewareExecutor(configs);
    //
    //         var consumerRecords = Fake.GetConsumerRecords(1, 100);
    //
    //         IConsumerContext context = new ConsumerContext(consumerRecords, new ReceivedContext("test"), null,
    //             CancellationToken.None);
    //         
    //         await middlewareExecutor.Execute(context, (_) => Task.CompletedTask);
    //
    //         var middlewares = new LinkedList<IMessageMiddleware>();
    //
    //         serviceCollection.AddMiddleware<DeadLetterHandler>(ServiceLifetime.Singleton);
    //         serviceCollection.AddMiddleware<CommitRecordsHandler>(ServiceLifetime.Singleton);
    //
    //         var provider = serviceCollection.BuildServiceProvider();
    //         var resolver = provider.GetRequiredService<IDependencyResolver>();
    //     }
    // }
}