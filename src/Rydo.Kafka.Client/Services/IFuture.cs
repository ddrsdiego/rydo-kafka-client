namespace Rydo.Kafka.Client.Services
{
    using System.Threading.Tasks;

    public interface IFuture
    {
        Task Run();
    }
}