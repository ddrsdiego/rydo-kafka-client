namespace Rydo.Kafka.Client.Exceptions
{
    using System;

    public class InvalidConsumerContractTypeException : Exception
    {
        public InvalidConsumerContractTypeException(string? handlerType)
            : base(CreateMessage(handlerType))
        {
        }

        private static string CreateMessage(string? handlerType) =>
            $"The consumer handler {handlerType} has not a contract defined. {handlerType} is configured correct?";
    }
}