namespace Rydo.Kafka.Client.Serializations.Extensions
{
    using Deserializers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal static class MessageDispatcherDeserializerExtensions
    {
        public static IServiceCollection AddDeserializers<TType>(this IServiceCollection services)
        {
            const string systemByte = "System.Byte[]";
            const string systemString = "System.String";

            var fullName = typeof(TType).FullName;

            return fullName is systemByte
                ? services.AddUtf8BytesDeserializer()
                : services.AddStringDeserializer();
        }

        private static IServiceCollection AddUtf8BytesDeserializer(this IServiceCollection services)
        {
            return services.AddSingleton<IMessageDispatcherDeserializer<string, byte[]>>(sp =>
            {
                IMessageDispatcherKeyDeserializer<string> keySerializer =
                    new MessageDispatcherKeyStringDeserializer(
                        sp.GetRequiredService<ILogger<MessageDispatcherKeyStringDeserializer>>());

                IMessageDispatcherValueDeserializer<byte[]> valueSerializer =
                    new MessageDispatcherValueUtf8BytesDeserializer(
                        sp.GetRequiredService<ILogger<MessageDispatcherValueUtf8BytesDeserializer>>());

                return new MessageDispatcherUtf8BytesDeserializer(keySerializer, valueSerializer);
            });
        }
        
        private static IServiceCollection AddStringDeserializer(this IServiceCollection services)
        {
            return services.AddSingleton<IMessageDispatcherDeserializer<string, string>>(sp =>
            {
                IMessageDispatcherKeyDeserializer<string> keySerializer =
                    new MessageDispatcherKeyStringDeserializer(
                        sp.GetRequiredService<ILogger<MessageDispatcherKeyStringDeserializer>>());

                IMessageDispatcherValueDeserializer<string> valueSerializer =
                    new MessageDispatcherValueStringDeserializer(
                        sp.GetRequiredService<ILogger<MessageDispatcherValueStringDeserializer>>());

                return new MessageDispatcherStringDeserializer(keySerializer, valueSerializer);
            });
        }
    }
}