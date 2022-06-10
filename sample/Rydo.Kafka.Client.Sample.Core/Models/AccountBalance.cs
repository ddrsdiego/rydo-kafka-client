namespace Rydo.Kafka.Client.Sample.Core.Models
{
    public class AccountBalance
    {
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; }

        public override string ToString() => $"Account Number: {AccountNumber} - Balance: {Balance}";
    }
}