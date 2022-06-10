namespace Rydo.Kafka.Client.Configurations.Extensions
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public static  class TopicConfigValidationExtensions
    {
        public static IEnumerable<string?> ValidateConfiguration(this object config)
        {
            var context = new ValidationContext(config);
            var errorsFound = new List<ValidationResult>();

            var validationPassed = Validator.TryValidateObject(config, context, errorsFound);
            foreach (var validationResult in errorsFound)
            {
                yield return validationResult.ErrorMessage;
            }
        }
    }
}