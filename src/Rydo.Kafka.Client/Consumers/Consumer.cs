namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.Logging;

    public interface IConsumer
    {
        string? Name { get; }
        
        void Subscribe(ConsumerContext<byte[], byte[]>? consumerContext);

        ValueTask<ConsumeResult<byte[], byte[]>?> ConsumeAsync(CancellationToken cancellationToken);

        ValueTask Commit(ConsumerRecord consumerRecord);
    }

    public class Consumer : IConsumer
    {
        private readonly ILogger<Consumer> _logger;
        private IConsumer<byte[], byte[]>? _consumer;
        private ConsumerContext<byte[], byte[]>? _consumerContext;

        public Consumer(ILogger<Consumer> logger)
        {
            _logger = logger;
        }

        public string? Name => _consumer?.Name;

        public void Subscribe(ConsumerContext<byte[], byte[]>? consumerContext)
        {
            _consumerContext = consumerContext;
        }

        public async ValueTask<ConsumeResult<byte[], byte[]>?> ConsumeAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    EnsureConsumer();
                    await Task.Delay(1, cancellationToken);
                    
                    return _consumer?.Consume(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (KafkaException e) when (e.Error.IsFatal)
                {
                    _logger.LogError(e, "Kafka Consumer fatal error occurred. Recreating consumer in 5 seconds");
                    
                    InvalidateConsumer();

                    await Task.Delay(5_000, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Kafka Consumer Error");
                }
            }
        }

        public ValueTask Commit(ConsumerRecord consumerRecord)
        {
            _consumer?.Commit(consumerRecord.ConsumeResult);
            return new ValueTask(Task.CompletedTask);
        }

        private void EnsureConsumer()
        {
            if (_consumer != null)
                return;
            
            _consumer = _consumerContext?.ConsumerBuilder
                .Build();
            
            _consumer?.Subscribe(_consumerContext?.ConsumerSpecification.Topic.Name);
        }

        private void InvalidateConsumer()
        {
            _consumer?.Close();
            _consumer = null;
        }
    }
}