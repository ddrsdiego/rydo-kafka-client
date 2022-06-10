namespace Rydo.Kafka.Client.Dispatchers.Extensions
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Constants;
    using Logging;
    using Logging.Message;
    using Microsoft.Extensions.Logging;

    internal static class DeliveryResultExtension
    {
        public static ProducerResponse GetResponse(
            this DeliveryResult<byte[], byte[]>? producerResult,
            ProducerRequest producerRequest)
        {
            return producerResult?.Status switch
            {
                PersistenceStatus.Persisted => new ProducerResponse(producerRequest,
                    PersistenceStatus.Persisted),
                PersistenceStatus.NotPersisted => new ProducerResponse(producerRequest,
                    PersistenceStatus.NotPersisted),
                PersistenceStatus.PossiblyPersisted => new ProducerResponse(producerRequest,
                    PersistenceStatus.PossiblyPersisted),

                _ => new ProducerResponse(producerRequest, PersistenceStatus.NotPersisted)
            };
        }

        public static async IAsyncEnumerable<ProducerResponse> GetResponseAsync(
            this DeliveryResult<byte[], byte[]>? producerResult,
            ProducerRequest producerRequest)
        {
            if (producerResult != null)
            {
                yield return producerResult.Status switch
                {
                    PersistenceStatus.Persisted => new ProducerResponse(producerRequest,
                        PersistenceStatus.Persisted),
                    PersistenceStatus.NotPersisted => new ProducerResponse(producerRequest,
                        PersistenceStatus.NotPersisted),
                    PersistenceStatus.PossiblyPersisted => new ProducerResponse(producerRequest,
                        PersistenceStatus.PossiblyPersisted),

                    _ => new ProducerResponse(producerRequest, PersistenceStatus.NotPersisted)
                };
            }

            await Task.CompletedTask;
        }

        public static void LogResultProducerMessage(
            this DeliveryResult<byte[], byte[]>? producerResult, ILogger logger)
        {
            byte[]? producerNameUtf8 = default;
            byte[]? environmentUtf8 = default;
            byte[]? cidUtf8 = default;

            producerResult?.Headers.TryGetLastBytes(MessageHeadersDefault.CorrelationId, out cidUtf8);
            producerResult?.Headers.TryGetLastBytes(MessageHeadersDefault.Env, out environmentUtf8);
            producerResult?.Headers.TryGetLastBytes(MessageHeadersDefault.Producer, out producerNameUtf8);

            var cid = Encoding.UTF8.GetString(cidUtf8!);
            var environment = Encoding.UTF8.GetString(environmentUtf8!);
            var producerName = Encoding.UTF8.GetString(producerNameUtf8!);

            var log = new KafkaConsumerOutLog(producerName, cid, environment, producerResult);

            logger.LogInformation($"[{KafkaClientLogField.LogType}] | {KafkaClientLogField.KafkaProducerOutLog}",
                log.Log,
                log);
        }
    }
}