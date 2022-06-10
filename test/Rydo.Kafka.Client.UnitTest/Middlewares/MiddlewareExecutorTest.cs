namespace Rydo.Kafka.Client.UnitTest.Middlewares
{
    using System;
    using System.Threading.Tasks;
    using Client.Middlewares;
    using Client.Middlewares.Extensions;
    using FakeData;
    using Microsoft.Extensions.DependencyInjection;
    using Shared;
    using Xunit;

    public class MiddlewareExecutorTest
    {
        [Fact]
        public void Should_Throws_Exception_When_ConsumerContext_Is_Null()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging().AddMiddlewares();
            
            var provider = serviceCollection.BuildServiceProvider();

            var middlewareExecutor = new MiddlewareExecutor(provider);
            var consumerRecords = Fake.GetConsumerRecords(1, 100);
            
            Assert.ThrowsAny<ArgumentNullException>(() =>
            {
                return middlewareExecutor.Execute(provider.CreateScope(), null, (_) => Task.CompletedTask);
            });
        }

        [Fact]
        public async Task Should_Execute_All_Middlewares()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging();
            serviceCollection.AddMiddlewares();
            serviceCollection.AddScoped(typeof(ConsumerHandlerTest));

            var provider = serviceCollection.BuildServiceProvider();
            var scope = provider.CreateScope();

            var middlewareExecutor = new MiddlewareExecutor(provider);

            var context = Fake.GetMessageConsumerContext(2, 1_000);

            await middlewareExecutor.Execute(scope, context, (_) => Task.CompletedTask)!;
        }
    }
}