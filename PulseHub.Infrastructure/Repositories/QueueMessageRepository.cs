using Microsoft.EntityFrameworkCore;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Interfaces;
using PulseHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PulseHub.Infrastructure.Repositories
{
    public class QueueMessageRepository : IQueueMessageRepository
    {
        private readonly PulseHubDbContext _context;

        public QueueMessageRepository(PulseHubDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QueueMessage>> GetAllAsync()
        {
            return await _context.QueueMessages
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<QueueMessage?> GetByIdAsync(Guid id)
        {
            return await _context.QueueMessages
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.QueueMessageId == id);
        }

        public async Task<IEnumerable<QueueMessage>> GetBySyncEventIdAsync(Guid syncEventId)
        {
            return await _context.QueueMessages
                .AsNoTracking()
                .Where(q => q.SyncEventId == syncEventId)
                .ToListAsync();
        }

        public async Task AddAsync(QueueMessage queueMessage)
        {
            await _context.QueueMessages.AddAsync(queueMessage);
        }

        public void Update(QueueMessage queueMessage)
        {
            _context.QueueMessages.Update(queueMessage);
        }

        public void Delete(QueueMessage queueMessage)
        {
            _context.QueueMessages.Remove(queueMessage);
        }
    }
}
