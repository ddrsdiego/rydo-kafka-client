namespace Rydo.Kafka.Client.Consumers.Extensions
{
    using System;
    using System.Text.Json;
    using Dispatchers;
    using MassTransit.Serialization;

    internal static class MessageKeyExtensions
    {
        public static object? GetMessageValue(this byte[] messageKey, Type contractType)
        {
            var envelope =
                JsonSerializer.Deserialize<KafkaEnvelope>(messageKey, SystemTextJsonMessageSerializer.Options);

            return JsonSerializer.Deserialize(envelope?.Payload, contractType,
                SystemTextJsonMessageSerializer.Options);

            // var json = Encoding.UTF8.GetString(messageKey);
            //
            // return JsonSerializer.Deserialize(json, contractType, SystemTextJsonMessageSerializer.Options);
        }
    }
}