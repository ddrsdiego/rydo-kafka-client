namespace Rydo.Kafka.Client.UnitTest.Consumers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Client.Consumers;
    using Client.Dispatchers;
    using Client.Middlewares.Extensions;
    using FakeData;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NSubstitute;
    using Xunit;

    public class KafkaListenerTest
    {
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly IHostApplicationLifetime _hostApplication;
        
        public KafkaListenerTest()
        {
            _messageDispatcher = Substitute.For<IMessageDispatcher>();
            _hostApplication = Substitute.For<IHostApplicationLifetime>();
        }
        
        [Fact]
        public void Test()
        {
            _hostApplication.ApplicationStopping.Returns(x => new CancellationToken());
            
            var services = new ServiceCollection();
            
            services.AddLogging();
            services.AddMiddlewares();
            services.AddSingleton(_hostApplication)
                .AddSingleton(_messageDispatcher);

            var consumerContext = Fake.GetConsumerContext();
            var provider = services.BuildServiceProvider();
            
            var kafkaLister = new KafkaListener("dummy-topic", consumerContext);
            
            kafkaLister.SetupDependencies(provider);
            Task.Run(() => kafkaLister.StartAsync(_hostApplication.ApplicationStopping));
            
            kafkaLister.EnqueueAsync(Fake.GetConsumeResult(Fake.GetDummyModels(1).First()));
        }
    }
}