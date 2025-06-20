using System.Threading.Tasks;

namespace PulseHub.Infrastructure.Messaging.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string message);
    }
}
