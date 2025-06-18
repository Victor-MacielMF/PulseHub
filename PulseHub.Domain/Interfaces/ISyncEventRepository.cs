using PulseHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseHub.Domain.Interfaces
{
    public interface ISyncEventRepository
    {
        Task<SyncEvent?> GetByIdAsync(Guid id);
        Task<IEnumerable<SyncEvent>> GetAllAsync();
        Task AddAsync(SyncEvent syncEvent);
        void Update(SyncEvent syncEvent);
        void Delete(SyncEvent syncEvent);
    }
}
