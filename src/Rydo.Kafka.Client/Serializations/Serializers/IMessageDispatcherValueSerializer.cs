namespace Rydo.Kafka.Client.Serializations.Serializers
{
    using Dispatchers;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public interface IMessageDispatcherValueSerializer<out TOut>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        TOut GetMessageValue(ProducerRequest request);
    }
}