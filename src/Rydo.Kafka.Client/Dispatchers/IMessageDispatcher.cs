namespace Rydo.Kafka.Client.Dispatchers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessageDispatcher
    {
        /// <summary>
        /// Produces a new message 
        /// </summary>
        /// <param name="topicName">The topic where the message wil be produced</param>
        /// <param name="messageValue">The message value</param>
        /// <param name="cancellationToken">A cancellation token to observe whilst waiting the returned task to complete.</param>
        /// <returns></returns>
        Task<ProducerResponse> SendAsync(string topicName, object messageValue,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a new message
        /// </summary>
        /// <param name="topicName">The topic where the message wil be produced</param>
        /// <param name="messageKey">The message key</param>
        /// <param name="messageValue">The message value</param>
        /// <param name="cancellationToken">A cancellation token to observe whilst waiting the returned task to complete.</param>
        /// <returns></returns>
        Task<ProducerResponse> SendAsync(string topicName, string messageKey, object messageValue,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">A cancellation token to observe whilst waiting the returned task to complete.</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        Task<IEnumerable<ProducerResponse>> BatchSendAsync(IProducerBatchRequest request,
            CancellationToken cancellationToken = default);
    }
}