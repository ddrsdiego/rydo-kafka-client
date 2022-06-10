namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Dispatchers;
    using Extensions;
    using Handlers;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Middlewares;
    using Serializations;

    internal sealed class KafkaListener : IKafkaListener<byte[], byte[]>, IAsyncDisposable
    {
        private int _faultCounter;

        private ILogger? _logger;
        private IServiceProvider? _serviceProvider;
        private IConsumer? _consumer;
        private IMiddlewareExecutor? _middlewareExecutor;
        private CancellationToken _cancellationToken;

        // private IConsumer<byte[], byte[]>? _consumer;

        private readonly Task _readerTask;
        private readonly Channel<ConsumeResult<byte[], byte[]>> _queue;
        private readonly ConsumerContext<byte[], byte[]>? _consumerContext;

        public KafkaListener(string topicName, ConsumerContext<byte[], byte[]> kafkaConsumerContext)
        {
            const int channelCapacity = 2_000;

            _consumerContext =
                kafkaConsumerContext ?? throw new ArgumentNullException(nameof(kafkaConsumerContext));

            TopicName = topicName;
            IsRunning = Task.FromResult(true);

            _faultCounter = 0;
            _cancellationToken = new CancellationToken();

            var channelOptions = new BoundedChannelOptions(channelCapacity)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            _queue = Channel.CreateBounded<ConsumeResult<byte[], byte[]>>(channelOptions);

            _readerTask = Task.Run(ReadFromChannel);
        }

        public string TopicName { get; }

        public int FaultCounter { get; }

        public Task<bool> IsRunning { get; set; }

        public void SetupDependencies(IServiceProvider? dependencyResolver)
        {
            _serviceProvider = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));

            _middlewareExecutor = new MiddlewareExecutor(dependencyResolver);

            _consumer = new Consumer(dependencyResolver.GetRequiredService<ILogger<Consumer>>());
            _logger = dependencyResolver.GetRequiredService<ILogger<KafkaListener>>();
            _cancellationToken = dependencyResolver.GetRequiredService<IHostApplicationLifetime>()
                .ApplicationStopping;
        }

        public async Task<bool> StartAsync(CancellationToken stoppingToken)
        {
            if (CancellationTokenIsInvalid())
                _cancellationToken = stoppingToken;

            var consumerConfig =
                JsonSerializer.Serialize(_consumerContext?.ConsumerConfig,
                    SystemTextJsonMessageSerializer.Options);

            _logger?.LogInformation(
                $"{KafkaClientLogField.LogType} Starting Topic Listener: {KafkaClientLogField.TopicName} | {KafkaClientLogField.ConsumerConfig}",
                KafkaClientLogType.StartingConsumer,
                _consumerContext?.ConsumerSpecification.Topic.Name,
                consumerConfig);

            // _consumer = _consumerContext?.ConsumerBuilder
            //     .Build();
            // _consumer?.Subscribe(_consumerContext?.ConsumerSpecification.Topic.Name);

            _consumer?.Subscribe(_consumerContext);

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<byte[], byte[]>? consumeResult = default;

                consumeResult = _consumer!.Consume(stoppingToken);
                if (consumeResult!.IsPartitionEOF)
                    continue;

                await EnqueueAsync(consumeResult, stoppingToken);
            }

            return IsRunning.Result;
        }

        private bool CancellationTokenIsInvalid() => _cancellationToken.IsCancellationRequested;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Task EnqueueAsync(ConsumeResult<byte[], byte[]> consumeResult,
            CancellationToken cancellationToken = default)
        {
            var writeTask = _queue.Writer.WriteAsync(consumeResult, cancellationToken);
            return writeTask.IsCompletedSuccessfully ? Task.CompletedTask : SlowWrite(writeTask);

            static async Task SlowWrite(ValueTask task) => await task;
        }

        public void IncrementFaultCounter() => Interlocked.Increment(ref _faultCounter);

        private async Task ReadFromChannel()
        {
            const int batchCapacity = 1_000;

            try
            {
                while (await _queue.Reader.WaitToReadAsync(_cancellationToken).ConfigureAwait(false))
                {
                    var counter = 0;
                    IConsumerRecords consumerRecords = new ConsumerRecords();

                    while (counter < batchCapacity && _queue.Reader.TryRead(out var consumeResult))
                    {
                        var consumerRecord = consumeResult.TryAdapterToConsumerRecord(_consumerContext, _logger);
                        if (consumerRecord.IsFailure)
                            continue;

                        consumerRecords.Add(consumerRecord.Value);
                        consumerRecord.Value.LogMessageInComing(consumerRecords.BatchId, _logger);

                        counter++;
                    }

                    var scope = _serviceProvider?.CreateScope();

                    var consumerContext =
                        CreateConsumerContext(_consumer, consumerRecords, scope, _cancellationToken);

                    await _middlewareExecutor?.Execute(scope, consumerContext, _ => Task.CompletedTask)!;
                }
            }
            catch (OperationCanceledException e) when (e.CancellationToken == _cancellationToken)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private MessageConsumerContext CreateConsumerContext(IConsumer? consumer,
            IConsumerRecords consumerRecords, IServiceScope? scope, CancellationToken cancellationToken)
        {
            var messageDispatcher = scope?.ServiceProvider.GetRequiredService<IMessageDispatcher>();
            IReceivedContext receivedContext = new ReceivedContext(consumer.Name);

            var context =
                new MessageConsumerContext(consumerRecords, _consumerContext, receivedContext, messageDispatcher,
                    consumer, cancellationToken);

            return context;
        }

        public async ValueTask DisposeAsync()
        {
            await _readerTask;
            _queue.Writer.Complete();
        }
    }
}