namespace Rydo.Kafka.Client.Dispatchers
{
    using System;
    using Confluent.Kafka;

    public readonly struct ProducerResponseError
    {
        internal ProducerResponseError(Error error)
        {
            Code = error.Code;
            Reason = error.Reason;
        }

        internal ProducerResponseError(ErrorCode code, string reason)
        {
            Code = code;
            Reason = reason;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public readonly ErrorCode Code;
        
        /// <summary>
        /// 
        /// </summary>
        public readonly string Reason;

        internal static ProducerResponseError NoError() => new ProducerResponseError(ErrorCode.NoError, string.Empty);
    }

    public readonly struct ProducerResponse
    {
        internal ProducerResponse(ProducerRequest request, PersistenceStatus persistenceStatus)
            : this(request, persistenceStatus, ProducerResponseError.NoError())
        {
        }

        private ProducerResponse(ProducerRequest request, PersistenceStatus persistenceStatus,
            ProducerResponseError error)
        {
            Error = error;
            Request = request;
            ResponseAt = DateTime.Now;
            PersistenceStatus = persistenceStatus;
            ElapsedTimeMs = ResponseAt.Subtract(Request.CreatedAt).Milliseconds;
        }

        /// <summary>
        /// Time elapsed in milliseconds between the creation of the request and the response provided by the broker
        /// </summary>
        public readonly int ElapsedTimeMs;

        /// <summary>
        /// Original request associated with response
        /// </summary>
        public readonly ProducerRequest Request;

        /// <summary>
        /// 
        /// </summary>
        public readonly PersistenceStatus PersistenceStatus;

        /// <summary>
        /// 
        /// </summary>
        public readonly ProducerResponseError Error;

        /// <summary>
        /// Date and time of the response provided by the broker
        /// </summary>
        public readonly DateTime ResponseAt;
    }
}