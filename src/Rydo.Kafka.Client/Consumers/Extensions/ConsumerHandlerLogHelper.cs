namespace Rydo.Kafka.Client.Consumers.Extensions
{
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;
    using Handlers;
    using Logging;

    internal static class ConsumerHandlerLogHelper
    {
        private const string StartingConsumerHandlerLogType = "consumer-handler-starting";
        private const string FinishingConsumerHandlerLogType = "consumer-handler-finishing";

        public static void LogFinishingConsumerHandler(this IConsumerHandler messageHandler, string handlerTypeName,
            ILogger logger,
            Stopwatch sw)
        {
            logger.LogInformation(
                $"[{KafkaClientLogField.LogType}] - [{KafkaClientLogField.HandlerId}] -  Finishing consumption by {KafkaClientLogField.ConsumerHandlerName} handler. Elapsed Time: {KafkaClientLogField.ElapsedMilliseconds} ms.",
                FinishingConsumerHandlerLogType,
                messageHandler.HandlerId,
                handlerTypeName,
                sw.ElapsedMilliseconds);
        }

        public static void LogStartingConsumerHandler(this IConsumerHandler messageHandler, string handlerTypeName,
            ILogger logger)
        {
            logger.LogDebug(
                $"[{KafkaClientLogField.LogType}] - [{KafkaClientLogField.HandlerId}] - Starting consumption by {KafkaClientLogField.ConsumerHandlerName} handler",
                StartingConsumerHandlerLogType,
                messageHandler.HandlerId,
                handlerTypeName);
        }
    }
}