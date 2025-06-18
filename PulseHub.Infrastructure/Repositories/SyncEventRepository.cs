using Microsoft.EntityFrameworkCore;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Interfaces;
using PulseHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseHub.Infrastructure.Repositories
{
    public class SyncEventRepository : ISyncEventRepository
    {
        private readonly PulseHubDbContext _context;

        public SyncEventRepository(PulseHubDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SyncEvent>> GetAllAsync()
        {
            return await _context.SyncEvents
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SyncEvent?> GetByIdAsync(Guid id)
        {
            return await _context.SyncEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.SyncEventId == id);
        }

        public async Task AddAsync(SyncEvent syncEvent)
        {
            await _context.SyncEvents.AddAsync(syncEvent);
        }

        public void Update(SyncEvent syncEvent)
        {
            _context.SyncEvents.Update(syncEvent);
        }

        public void Delete(SyncEvent syncEvent)
        {
            _context.SyncEvents.Remove(syncEvent);
        }
    }
}
