namespace Rydo.Kafka.Client.Consumers.Extensions
{
    using System;
    using System.Collections.Immutable;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Confluent.Kafka;
    using Constants;
    using CSharpFunctionalExtensions;
    using Exceptions;
    using Logging;
    using Microsoft.Extensions.Logging;

    public static class ConsumerRecordExtension
    {
        private const string ExtractToConsumerRecordError = "ERROR_EXTRACT_CONSUMER_RECORD";
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<ConsumerRecord> TryAdapterToConsumerRecord(this ConsumeResult<byte[], byte[]> consumeResult,
            ConsumerContext<byte[], byte[]>? consumerContext, ILogger? logger)
        {
            if (consumerContext?.ContractType == null)
                throw new InvalidConsumerContractTypeException(consumerContext?.HandlerType?.FullName);

            try
            {
                var headers = ImmutableDictionary<string, object>.Empty;

                if (consumeResult.Message.Headers.TryGetLastBytes(MessageHeadersDefault.Signature,
                        out var signatureValue))
                    headers = headers.Add(MessageHeadersDefault.Signature, Encoding.UTF8.GetString(signatureValue));

                if (consumeResult.Message.Headers.TryGetLastBytes(MessageHeadersDefault.Env, out var envValue))
                    headers = headers.Add(MessageHeadersDefault.Env, Encoding.UTF8.GetString(envValue));

                if (consumeResult.Message.Headers.TryGetLastBytes(MessageHeadersDefault.Producer,
                        out var producerValue))
                    headers = headers.Add(MessageHeadersDefault.Producer, Encoding.UTF8.GetString(producerValue));

                if (consumeResult.Message.Headers.TryGetLastBytes(MessageHeadersDefault.CorrelationId,
                        out var correlationIdValue))
                    headers = headers.Add(MessageHeadersDefault.CorrelationId,
                        Encoding.UTF8.GetString(correlationIdValue));

                var contract = consumeResult.Message.Value.GetMessageValue(consumerContext.ContractType);
                return Result.Success(new ConsumerRecord(contract, consumeResult, consumerContext, headers));
            }
            catch (Exception e)
            {
                logger.LogError(e, $"{KafkaClientLogField.LogType}",
                    ExtractToConsumerRecordError);
                
                return Result.Failure<ConsumerRecord>(e.ToString());
            }
        }
    }
}