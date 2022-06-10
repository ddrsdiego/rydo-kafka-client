namespace Rydo.Kafka.Client.Dispatchers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Exceptions;
    using Extensions;
    using Microsoft.Extensions.Logging;
    using Producers;

    internal sealed class KafkaMessageDispatcher : IMessageDispatcher
    {
        private readonly IProducerErrorHandler _producerErrorHandler;
        private readonly ILogger<KafkaMessageDispatcher> _logger;
        private readonly IKafkaMessageFactory<byte[]> _kafkaMessageFactory;
        private readonly IProducerContextContainer<byte[], byte[]> _producerContextContainer;

        public KafkaMessageDispatcher(ILogger<KafkaMessageDispatcher> logger,
            IProducerContextContainer<byte[], byte[]> producerContextContainer,
            IKafkaMessageFactory<byte[]> kafkaMessageFactory,
            IProducerErrorHandler producerErrorHandler)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _kafkaMessageFactory = kafkaMessageFactory ?? throw new ArgumentNullException(nameof(kafkaMessageFactory));
            _producerErrorHandler = producerErrorHandler;
            _producerContextContainer = producerContextContainer ??
                                        throw new ArgumentNullException(nameof(producerContextContainer));
        }

        public Task<ProducerResponse> SendAsync(string topicName, object messageValue,
            CancellationToken cancellationToken = default)
        {
            return SendAsync(ProducerRequest.Create(topicName, messageValue), cancellationToken);
        }

        public Task<ProducerResponse> SendAsync(string topicName, string messageKey, object messageValue,
            CancellationToken cancellationToken = default)
        {
            return SendAsync(ProducerRequest.Create(topicName, messageKey, messageValue), cancellationToken);
        }

        private async Task<ProducerResponse> SendAsync(ProducerRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var batch = ProducerBatchRequest.Create(request.Topic.Name);
                batch.Add(request.MessageKey, request.MessageValue);

                var responses = await BatchSendAsync(batch, cancellationToken);
                foreach (var response in responses)
                {
                    if (response.Request.MessageKey == request.MessageKey)
                        return new ProducerResponse(request, response.PersistenceStatus);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
                return new ProducerResponse(request, PersistenceStatus.PossiblyPersisted);
            }

            return new ProducerResponse(request, PersistenceStatus.PossiblyPersisted);
        }

        public async Task<IEnumerable<ProducerResponse>> BatchSendAsync(IProducerBatchRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request.Count <= 0)
                throw new InvalidOperationException();

            if (!_producerContextContainer.TryGetProducer(request.TopicName, out var producerContext))
                throw new ProducerContextNotFoundException(request.TopicName);

            var producerTasks =
                new Dictionary<ProducerRequest, Task<DeliveryResult<byte[], byte[]>>>(request.Items.Count);

            using (var producer = producerContext?.ProducerBuilder
                       .Build())
            {
                foreach (var dispatcherRequest in request.Items)
                {
                    StartMessageProduction(request, producer, dispatcherRequest, producerTasks, cancellationToken);
                }

                producer?.Flush(cancellationToken);
            }

            var dispatcherResponses = new List<ProducerResponse>(producerTasks.Count);
            foreach (var (req, rsp) in producerTasks)
            {
                try
                {
                    if (!rsp.IsCompletedSuccessfully)
                        await rsp;

                    var deliveryResult = rsp.Result;
                    dispatcherResponses.Add(deliveryResult.GetResponse(req));

                    deliveryResult.LogResultProducerMessage(_logger);
                }
                catch (ProduceException<byte[], byte[]> e)
                {
                    // _producerHandlerError.HandleProducerError(e, req, dispatcherResponses);
                }
                catch (Exception e)
                {
                    // _producerHandlerError.HandleProducerError(e, req, dispatcherResponses);
                }
            }

            request.Dispose();
            producerTasks.Clear();
            
            return dispatcherResponses;
        }

        private void StartMessageProduction(IProducerBatchRequest request, IProducer<byte[], byte[]>? producer,
            ProducerRequest producerRequest,
            IDictionary<ProducerRequest, Task<DeliveryResult<byte[], byte[]>>> producerTasks,
            CancellationToken cancellationToken)
        {
            if (producer == null)
                return;

            var kafkaMessage = _kafkaMessageFactory.CreateKafkaMessage(producerRequest);
            if (kafkaMessage.IsFailure)
                return;

            var task = producer.ProduceAsync(request.TopicName, kafkaMessage.Value, cancellationToken);
            producerTasks.Add(producerRequest, task);
        }
    }
}