namespace Rydo.Kafka.Client.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Consumers;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    internal sealed class KafkaIntegrationHostedService : BackgroundService
    {
        private const int FaultCounterLimit = 3;

        private readonly ILogger<KafkaIntegrationHostedService> _logger;
        private readonly IKafkaListenerContainer<byte[], byte[]> _kafkaListenerContainer;
        private readonly IConsumerContextContainer<byte[], byte[]> _consumerContextContainer;

        public KafkaIntegrationHostedService(ILogger<KafkaIntegrationHostedService> logger,
            IKafkaListenerContainer<byte[], byte[]> kafkaListenerContainer,
            IConsumerContextContainer<byte[], byte[]> consumerContextContainer)
        {
            _logger = logger;
            _kafkaListenerContainer = kafkaListenerContainer;
            _consumerContextContainer = consumerContextContainer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5_000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var (topicName, kafkaListener) in _kafkaListenerContainer.Listeners)
                {
                    try
                    {
                        if (!kafkaListener.IsRunning.IsCompleted)
                        {
                            continue;
                        }

                        if (!_consumerContextContainer.TryGetConsumerContext(topicName, out var consumerContext))
                        {
                            continue;
                        }

                        if (!await kafkaListener.IsRunning)
                        {
                            // DEFINE STRATEGY TO STOP THE LISTENER
                            continue;
                        }

                        kafkaListener.IsRunning =
                            Task.Run(async () => await kafkaListener.StartAsync(stoppingToken), stoppingToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, kafkaListener.TopicName);

                        kafkaListener.IncrementFaultCounter();
                        kafkaListener.IsRunning = Task.FromResult(CheckFaultCounterHasReachedLimit(kafkaListener));
                    }
                }

                await Task.Delay(1_000, stoppingToken);
            }
        }

        private static bool CheckFaultCounterHasReachedLimit(IKafkaListener<byte[], byte[]> kafkaListener) =>
            kafkaListener.FaultCounter <= FaultCounterLimit;
    }
}