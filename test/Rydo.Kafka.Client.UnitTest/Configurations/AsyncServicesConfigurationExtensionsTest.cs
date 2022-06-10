namespace Rydo.Kafka.Client.UnitTest.Configurations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Client.Configurations;
    using Client.Consumers;
    using Constants;
    using Extensions;
    using FluentAssertions;
    using Handlers;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Producers;
    using Shared;
    using Xunit;

    [TopicConsumer("ACCOUNT-CREATED")]
    public class AccountCreatedConsumerHandlerTest : ConsumerHandler<DummyModel>
    {
        public override Task Consumer(MessageConsumerContext context) => Task.CompletedTask;
    }

    public class AsyncServicesConfigurationExtensionsTest
    {
        [Fact]
        public void Should_Add_Valid_Producer_Topic()
        {
            const string topicName = "ORDER-CREATED";

            var dic = new Dictionary<string, string>
            {
                {"Mode", "MemoryConfigProvider"},
                {"AsyncServices:Topics:0:Name", topicName},
                {"AsyncServices:Topics:0:Direction", TopicDirections.Producer},
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(dic)
                .Build();

            var services = new ServiceCollection();
            services.AddLogging().AddAsyncServices(configuration);

            var serviceProvider = services.BuildServiceProvider();

            var topicConfigContextContainer = serviceProvider.GetRequiredService<ITopicConfigContextContainer>();
            var kafkaProducerContextContainer =
                serviceProvider.GetRequiredService<IProducerContextContainer<byte[], byte[]>>();

            topicConfigContextContainer.Entries.Count.Should().Be(1);

            var hasFoundConfig = topicConfigContextContainer.Entries.TryGetValue(topicName, out var topicDefinition);
            var hasFoundProducerContext =
                kafkaProducerContextContainer.TryGetProducer(topicName, out var producerContext);

            hasFoundConfig.Should().BeTrue();
            hasFoundProducerContext.Should().BeTrue();

            topicDefinition?.TopicName.Should().Be(topicName.ToUpperInvariant());
            topicDefinition?.Direction.Should().Be(TopicDirections.Producer);

            producerContext?.ProducerSpecification.Topic.Name.Should().Be(topicName.ToUpperInvariant());
        }

        [Fact]
        public void Should_Add_More_Than_One_Valid_Producer_Topic()
        {
            const string orderCreatedTopicName = "order-created";
            const string accountCreatedTopicName = "Account-Created";

            var dic = new Dictionary<string, string>
            {
                {"Mode", "MemoryConfigProvider"},

                {"AsyncServices:Topics:0:Name", orderCreatedTopicName},
                {"AsyncServices:Topics:0:Direction", "producer"},

                {"AsyncServices:Topics:1:Name", accountCreatedTopicName},
                {"AsyncServices:Topics:1:Direction", "both"},
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(dic)
                .Build();

            var services = new ServiceCollection();
            services.AddLogging().AddAsyncServices(configuration, typeof(AccountCreatedConsumerHandlerTest));

            var serviceProvider = services.BuildServiceProvider();

            var topicConfigContextContainer = serviceProvider.GetRequiredService<ITopicConfigContextContainer>();
            var kafkaProducerContextContainer =
                serviceProvider.GetRequiredService<IProducerContextContainer<byte[], byte[]>>();

            topicConfigContextContainer.Entries.Count.Should().Be(2);

            var hasFoundConfig =
                topicConfigContextContainer.Entries.TryGetValue(accountCreatedTopicName.ToUpperInvariant(),
                    out var topicDefinition);
            var hasFoundProducerContext =
                kafkaProducerContextContainer.TryGetProducer(orderCreatedTopicName, out var producerContext);

            hasFoundConfig.Should().BeTrue();
            hasFoundProducerContext.Should().BeTrue();

            topicDefinition?.TopicName.Should().Be(accountCreatedTopicName.ToUpperInvariant());
            topicDefinition?.Direction.Should().Be(TopicDirections.Both);

            producerContext?.ProducerSpecification.Topic.Name.Should().Be(orderCreatedTopicName.ToUpperInvariant());
        }

        [Fact]
        public void Should_Add_Valid_Consumer_Topic()
        {
            const string accountCreatedTopicName = "Account-Created";

            var dic = new Dictionary<string, string>
            {
                {"Mode", "MemoryConfigProvider"},

                {"AsyncServices:Topics:0:Name", accountCreatedTopicName},
                {"AsyncServices:Topics:0:Direction", "consumer"},
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(dic)
                .Build();

            var services = new ServiceCollection();
            services.AddLogging().AddAsyncServices(configuration, typeof(AccountCreatedConsumerHandlerTest));

            var serviceProvider = services.BuildServiceProvider();

            var topicConfigContextContainer = serviceProvider.GetRequiredService<ITopicConfigContextContainer>();
            var kafkaConsumerContextContainer =
                serviceProvider.GetRequiredService<IConsumerContextContainer<byte[], byte[]>>();

            topicConfigContextContainer.Entries.Count.Should().Be(1);

            var hasFoundConfig =
                topicConfigContextContainer.Entries.TryGetValue(accountCreatedTopicName.ToUpperInvariant(),
                    out var topicDefinition);
            var hasFoundConsumerContext =
                kafkaConsumerContextContainer.TryGetConsumerContext(accountCreatedTopicName, out var producerContext);

            hasFoundConfig.Should().BeTrue();
            hasFoundConsumerContext.Should().BeTrue();

            topicDefinition?.TopicName.Should().Be(accountCreatedTopicName.ToUpperInvariant());
            topicDefinition?.Direction.Should().Be(TopicDirections.Consumer);

            producerContext?.ConsumerSpecification.Topic.Name.Should().Be(accountCreatedTopicName.ToUpperInvariant());
        }

        [Fact]
        public void Should_Add_Valid_Consumer_Topic_With_DQL_Default()
        {
            const string accountCreatedTopicName = "Account-Created";

            var dic = new Dictionary<string, string>
            {
                {"Mode", "MemoryConfigProvider"},

                {"AsyncServices:Topics:0:Name", accountCreatedTopicName},
                {"AsyncServices:Topics:0:Direction", "consumer"},
                {"AsyncServices:Topics:0:DeadLetterPolicyName", "Default"},

                {"AsyncServices:DeadLetterPolicies:Default:Retry:Attempts", "10"},
                {"AsyncServices:DeadLetterPolicies:Default:Retry:Interval", "5s"},
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(dic)
                .Build();

            var services = new ServiceCollection();
            services.AddLogging().AddAsyncServices(configuration, typeof(AccountCreatedConsumerHandlerTest));

            var serviceProvider = services.BuildServiceProvider();

            var topicConfigContextContainer = serviceProvider.GetRequiredService<ITopicConfigContextContainer>();
            var kafkaConsumerContextContainer =
                serviceProvider.GetRequiredService<IConsumerContextContainer<byte[], byte[]>>();

            topicConfigContextContainer.Entries.Count.Should().Be(1);

            var hasFoundConfig =
                topicConfigContextContainer.Entries.TryGetValue(accountCreatedTopicName.ToUpperInvariant(),
                    out var topicDefinition);
            var hasFoundConsumerContext =
                kafkaConsumerContextContainer.TryGetConsumerContext(accountCreatedTopicName,
                    out var kafkaConsumerContext);

            hasFoundConfig.Should().BeTrue();
            hasFoundConsumerContext.Should().BeTrue();

            topicDefinition?.TopicName.Should().Be(accountCreatedTopicName.ToUpperInvariant());
            topicDefinition?.Direction.Should().Be(TopicDirections.Consumer);

            kafkaConsumerContext?.ConsumerSpecification.Topic.Name.Should()
                .Be(accountCreatedTopicName.ToUpperInvariant());
            kafkaConsumerContext?.ConsumerSpecification.DeadLetter.Should().NotBeNull();
            kafkaConsumerContext?.ConsumerSpecification.DeadLetter?.Retry.Should().NotBeNull();
            kafkaConsumerContext?.ConsumerSpecification.DeadLetter?.Retry.Attempts.Should().Be(10);
        }
    }
}