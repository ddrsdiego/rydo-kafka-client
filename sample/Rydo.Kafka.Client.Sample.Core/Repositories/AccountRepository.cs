namespace Rydo.Kafka.Client.Sample.Core.Repositories
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Models;
    using Microsoft.Extensions.Logging;

    public interface IAccountRepository
    {
        Task RegisterBalance(AccountBalance accountBalance);
    }

    public sealed class AccountRepository : IAccountRepository
    {
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(ILogger<AccountRepository> logger)
        {
            _logger = logger;
        }

        public async Task RegisterBalance(AccountBalance accountBalance)
        {
            _logger.LogInformation(
                $"Registrando {nameof(AccountBalance)} -> {JsonSerializer.Serialize(accountBalance)} - {DateTime.Now}");
            await Task.Delay(250);
        }
    }
}