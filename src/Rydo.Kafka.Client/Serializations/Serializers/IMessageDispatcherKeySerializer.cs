namespace Rydo.Kafka.Client.Serializations.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public interface IMessageDispatcherKeySerializer<out TOut>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        TOut GetMessageKey(Dispatchers.ProducerRequest request);
    }
}