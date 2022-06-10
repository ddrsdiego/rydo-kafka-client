namespace Rydo.Kafka.Client.Benchmark
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using Confluent.Kafka;
    using Consumers;
    using Consumers.Extensions;
    using FakeData;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    [MemoryDiagnoser]
    [NativeMemoryProfiler]
    public class ConsumerCommitHandlerBenchmark
    {
        private const int Offsets = 20_000;
        private IConsumerRecords? _consumerRecords;
        private ServiceProvider? _serviceProvider;

        [GlobalSetup]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            serviceCollection.AddScoped(sp => new ConsumerCommitHandlerDictionary(
                sp.GetRequiredService<ILogger<ConsumerCommitHandlerDictionary>>()));

            serviceCollection.AddScoped(sp => new ConsumerCommitHandlerLinq(
                sp.GetRequiredService<ILogger<ConsumerCommitHandlerLinq>>()));

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _consumerRecords = Fake.GetConsumerRecords(50, Offsets);
        }

        [Benchmark]
        public async Task ConsumerCommitHandlerDictionary()
        {
            using var scope = _serviceProvider?.CreateScope();

            var consumer =
                (ConsumerCommitHandlerDictionary) scope!.ServiceProvider.GetRequiredService(
                    typeof(ConsumerCommitHandlerDictionary));

            await consumer.Commit(_consumerRecords);
        }

        [Benchmark]
        public async Task ConsumerCommitHandlerLinq()
        {
            using var scope = _serviceProvider?.CreateScope();

            var consumer =
                (ConsumerCommitHandlerLinq) scope!.ServiceProvider.GetRequiredService(
                    typeof(ConsumerCommitHandlerLinq));

            await consumer.Commit(_consumerRecords);
        }
    }

    internal sealed class ConsumerCommitHandlerDictionary
    {
        private const string ErrorCommitConsumerRecord = "ERROR_COMMIT_CONSUMER_RECORD";

        private IConsumer<byte[], byte[]>? _consumer;
        private readonly ILogger<ConsumerCommitHandlerDictionary> _logger;
        private readonly Dictionary<int, ConsumerRecord>? _consumerRecordsToCommit;

        public ConsumerCommitHandlerDictionary(ILogger<ConsumerCommitHandlerDictionary> logger)
        {
            _logger = logger;
            _consumerRecordsToCommit = new Dictionary<int, ConsumerRecord>();
        }

        public void SetConsumer(IConsumer<byte[], byte[]>? consumer) =>
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));

        public ValueTask Commit(IConsumerRecords? consumerRecords)
        {
            if (!consumerRecords.Any())
                return new ValueTask();

            foreach (var consumerRecord in consumerRecords)
            {
                if (!_consumerRecordsToCommit!.TryGetValue(consumerRecord.Partition, out var currentConsumerRecord))
                    _consumerRecordsToCommit.Add(consumerRecord.Partition, consumerRecord);
                else
                {
                    if (consumerRecord.Offset <= currentConsumerRecord.Offset)
                        continue;

                    _consumerRecordsToCommit.Remove(consumerRecord.Partition);
                    _consumerRecordsToCommit.Add(consumerRecord.Partition, consumerRecord);
                }
            }

            if (_consumerRecordsToCommit?.Count <= 0) return new ValueTask();

            foreach (var (_, consumerRecord) in _consumerRecordsToCommit!)
            {
                try
                {
                    consumerRecord.Commit(_consumer);
                    consumerRecord.LogMessageCommit(consumerRecords.BatchId, _logger);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"{KafkaClientLogField.LogType}",
                        ErrorCommitConsumerRecord);
                }
            }

            return new ValueTask(Task.CompletedTask);
        }

        public void Dispose()
        {
            _consumerRecordsToCommit?.Clear();
        }
    }

    internal sealed class ConsumerCommitHandlerLinq
    {
        private const string ErrorCommitConsumerRecord = "ERROR_COMMIT_CONSUMER_RECORD";

        private IConsumer<byte[], byte[]>? _consumer;

        private readonly HashSet<int> _partitionsToCommit;
        private readonly ILogger<ConsumerCommitHandlerLinq> _logger;
        private List<ConsumerRecord>? _consumerRecordsToCommit;

        public ConsumerCommitHandlerLinq(ILogger<ConsumerCommitHandlerLinq> logger)
        {
            _partitionsToCommit = new HashSet<int>();

            _logger = logger;
            _consumerRecordsToCommit = new List<ConsumerRecord>();
        }

        public void SetConsumer(IConsumer<byte[], byte[]>? consumer) =>
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));

        public ValueTask Commit(IConsumerRecords? consumerRecords)
        {
            if (!consumerRecords.Any())
                return new ValueTask();

            foreach (var consumerRecord in consumerRecords)
                _partitionsToCommit.Add(consumerRecord.Partition);

            _consumerRecordsToCommit = new List<ConsumerRecord>(_partitionsToCommit.Count);

            foreach (var partition in _partitionsToCommit)
            {
                var lastConsumerRecord = consumerRecords
                    .Where(x => x.Partition == partition)
                    .OrderByDescending(x => x.Offset).First();

                _consumerRecordsToCommit.Add(lastConsumerRecord);
            }

            foreach (var lastConsumerRecord in _consumerRecordsToCommit)
            {
                try
                {
                    lastConsumerRecord.Commit(_consumer);
                    lastConsumerRecord.LogMessageCommit(consumerRecords.BatchId, _logger);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "");
                }
            }

            return new ValueTask(Task.CompletedTask);
        }

        public void Dispose()
        {
            _partitionsToCommit?.Clear();
            _consumerRecordsToCommit?.Clear();
        }
    }
}