namespace Rydo.Kafka.Client.UnitTest.Middlewares.Consumers
{
    using System.Threading.Tasks;
    using Client.Middlewares.Consumers;
    using Client.Middlewares.Extensions;
    using FakeData;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Xunit;

    public class OffsetCommitManagerMiddlewareTest
    {
        private readonly ILogger<OffsetCommitManagerMiddleware> _logger;

        public OffsetCommitManagerMiddlewareTest()
        {
            _logger = Substitute.For<ILogger<OffsetCommitManagerMiddleware>>();
        }

        [Fact]
        public async Task Test()
        {
            //arrange
            var services = new ServiceCollection();
            services.AddLogging().AddMiddlewares();
            
            var sut = new OffsetCommitManagerMiddleware(_logger);
            var messageConsumerContext = Fake.GetMessageConsumerContext(2, 1_000);
            
            //act
            await sut.Invoke(messageConsumerContext, (_) => Task.CompletedTask)!;

            //assert
        }
    }
}