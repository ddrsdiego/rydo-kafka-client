namespace Rydo.Kafka.Client.UnitTest.Consumers
{
    using System.Threading;
    using Client.Consumers;
    using FakeData;
    using Microsoft.Extensions.Logging;
    using Xunit;

    public class ConsumerTest
    {
        private readonly ILogger<Consumer> _logger;

        public ConsumerTest()
        {
            _logger = NSubstitute.Substitute.For<ILogger<Consumer>>();
            _logger = NSubstitute.Substitute.For<ILogger<Consumer>>();
        }

        [Fact]
        public void Test()
        {
            //arrange
            var sut = new Consumer(_logger);
            
            sut.Subscribe(Fake.GetConsumerContext());
            
            //act
            var a = sut.Consume(CancellationToken.None);
            //assert
        }
    }
}