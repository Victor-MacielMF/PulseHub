using System.Threading.Tasks;

namespace PulseHub.Consumer.MercadoLivre.Messaging.Interfaces
{
    public interface IMercadoLivreMessageProcessor
    {
        Task<bool> ProcessMessageAsync(string message);
    }
}
