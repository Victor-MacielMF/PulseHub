using System.Threading.Tasks;

namespace PulseHub.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
