using PulseHub.Domain.Interfaces;
using PulseHub.Infrastructure.Data;
using System.Threading.Tasks;

public class UnitOfWork : IUnitOfWork
{
    private readonly PulseHubDbContext _context;

    public UnitOfWork(PulseHubDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
