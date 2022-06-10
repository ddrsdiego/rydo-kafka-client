namespace Rydo.Kafka.Client.Serializations.Extensions
{
    using Dispatchers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serializers;

    internal static class MessageDispatcherSerializerExtensions
    {
        public static IServiceCollection AddSerializers<TType>(this IServiceCollection services)
        {
            const string systemByte = "System.Byte[]";
            const string systemString = "System.String";

            var fullName = typeof(TType).FullName;

            return fullName is systemByte
                ? services.AddUtf8BytesSerializer()
                : services.AddStringSerializer();
        }

        private static IServiceCollection AddUtf8BytesSerializer(this IServiceCollection services)
        {
            services.AddSingleton<IKafkaMessageFactory<byte[]>, KafkaUtf8BytesMessageFactory>();
            
            return services.AddSingleton<IMessageDispatcherSerializer<byte[], byte[]>>(sp =>
            {
                IMessageDispatcherKeySerializer<byte[]> keySerializer =
                    new MessageDispatcherKeyUtf8BytesSerializer(
                        sp.GetRequiredService<ILogger<MessageDispatcherKeyUtf8BytesSerializer>>());

                IMessageDispatcherValueSerializer<byte[]> valueSerializer =
                    new MessageDispatcherValueUtf8BytesSerializer(
                        sp.GetRequiredService<ILogger<MessageDispatcherValueUtf8BytesSerializer>>());

                return new MessageDispatcherUtf8BytesSerializer(keySerializer, valueSerializer);
            });
        }
        
        private static IServiceCollection AddStringSerializer(this IServiceCollection services)
        {
            services.AddSingleton<IKafkaMessageFactory<string>, KafkaStringMessageFactory>();
            
            return services.AddSingleton<IMessageDispatcherSerializer<string, string>>(sp =>
            {
                IMessageDispatcherKeySerializer<string> keySerializer =
                    new MessageDispatcherKeyStringSerializer(
                        sp.GetRequiredService<ILogger<MessageDispatcherKeyStringSerializer>>());

                IMessageDispatcherValueSerializer<string> valueSerializer =
                    new MessageDispatcherValueStringSerializer(
                        sp.GetRequiredService<ILogger<MessageDispatcherValueStringSerializer>>());

                return new MessageDispatcherStringSerializer(keySerializer, valueSerializer);
            });
        }
    }
}