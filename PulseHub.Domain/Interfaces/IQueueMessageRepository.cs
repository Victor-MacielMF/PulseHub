using PulseHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseHub.Domain.Interfaces
{
    public interface IQueueMessageRepository
    {
        Task<QueueMessage?> GetByIdAsync(Guid id);
        Task<IEnumerable<QueueMessage>> GetAllAsync();
        Task AddAsync(QueueMessage queueMessage);
        void Update(QueueMessage queueMessage);
        void Delete(QueueMessage queueMessage);
    }
}
