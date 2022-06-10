namespace Rydo.Kafka.Client.Dispatchers.Extensions
{
    using System;
    using System.Globalization;
    using System.Text;
    using Confluent.Kafka;
    using Constants;
    using Microsoft.Extensions.Logging;

    internal static class MessageMetadataExtension
    {
        private static string? _applicationName;
        private static string? _aspnetCoreEnvironment;
        
        private static string? ProducerName
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationName))
                    _applicationName =
                        Environment.GetEnvironmentVariable(EnvironmentsVariables.ApplicationName);
                
                return _applicationName;
            }
        }
        
        private static string? AspnetCoreEnvironment
        {
            get
            {
                if (string.IsNullOrEmpty(_aspnetCoreEnvironment))
                    _aspnetCoreEnvironment =
                        Environment.GetEnvironmentVariable(EnvironmentsVariables.AspnetCoreEnvironment);
                
                return _aspnetCoreEnvironment;
            }
        }
        
        public static void FillDefaultHeaders(this MessageMetadata kafkaMessage,
            ProducerRequest producerRequest, ILogger logger)
        {
            const string contentType = "application/json";

            kafkaMessage.TryAddHeader(MessageHeadersDefault.ContentType, contentType, logger);
            kafkaMessage.TryAddHeader(MessageHeadersDefault.Env, AspnetCoreEnvironment, logger);
            kafkaMessage.TryAddHeader(MessageHeadersDefault.Producer, ProducerName, logger);
            kafkaMessage.TryAddHeader(MessageHeadersDefault.ProducedAt,
                producerRequest.CreatedAt.ToString(CultureInfo.InvariantCulture), logger);
            kafkaMessage.TryAddHeader(MessageHeadersDefault.CorrelationId, producerRequest.MessageKey, logger);
        }

        public static void TryAddHeader(this MessageMetadata kafkaMessage, string keyHeader, string? valueHeader,
            ILogger logger)
        {
            if (!string.IsNullOrEmpty(valueHeader))
                kafkaMessage.Headers.Add(keyHeader, Encoding.UTF8.GetBytes(valueHeader));
            else
                logger.LogWarning("");
        }
    }
}