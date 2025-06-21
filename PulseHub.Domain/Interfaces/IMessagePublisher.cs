using System.Threading.Tasks;

namespace PulseHub.Domain.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string message, string channel);
    }
}
