namespace Rydo.Kafka.Client.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.Json;
    using Configurations;
    using Confluent.Kafka;
    using Serializations;

    public readonly struct ConsumerRecord
    {
        private readonly byte[] _key;
        private readonly byte[] _value;
        private readonly object? _messageValue;
        private readonly IDictionary<string, object> _headers;
        

        internal ConsumerRecord(object? messageValue, ConsumeResult<byte[], byte[]> consumeResult,
            ConsumerContext<byte[], byte[]> consumerContext, IDictionary<string, object> headers)
        {
            if (consumerContext == null) throw new ArgumentNullException(nameof(consumerContext));

            _messageValue = messageValue ?? throw new ArgumentNullException(nameof(messageValue));
            ConsumeResult = consumeResult ?? throw new ArgumentNullException(nameof(consumeResult));

            _key = consumeResult.Message.Key;
            _value = consumeResult.Message.Value;

            Offset = consumeResult.Offset.Value;
            Partition = consumeResult.Partition.Value;
            TopicName = consumeResult.Topic.ToUpperInvariant();

            Id = $"{TopicName}-{Partition}-{Offset}";

            DeadLetterSpec = consumerContext.ConsumerSpecification.DeadLetter;
            _headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }

        public readonly string TopicName;

        internal readonly string Id;

        internal readonly DeadLetterSpecification? DeadLetterSpec;

        internal readonly ConsumeResult<byte[], byte[]> ConsumeResult;
        
        public readonly long Offset;

        public readonly int Partition;

        public IDictionary<string, object> Headers => _headers.ToImmutableDictionary();

        /// <summary>
        /// Get the raw message contained in the Value field
        /// </summary>
        /// <typeparam name="T">The target type of the JSON contains in value field.</typeparam>
        /// <returns>A T representation of the JSON value.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? Value<T>()
        {
            if (_value is null) return default;

            try
            {
                return (T) _messageValue!;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? ValueAsJsonString()
        {
            if (_value is null) return default;

            try
            {
                if (_value is not { } utf8Json)
                    return default;

                var envelope =
                    JsonSerializer.Serialize(_messageValue, SystemTextJsonMessageSerializer.Options);

                return envelope;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Key() => _key is not { } utf8Json ? string.Empty : Encoding.UTF8.GetString(utf8Json);

        public override string ToString() => Id;

        public void Commit(IConsumer<byte[], byte[]>? consumer)
        {
            if (consumer == null) throw new ArgumentNullException(nameof(consumer));
            consumer.Commit(ConsumeResult);
        }
    }
}