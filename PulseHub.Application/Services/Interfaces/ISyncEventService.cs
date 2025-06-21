using PulseHub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseHub.Application.Services.Interfaces
{
    public interface ISyncEventService
    {
        Task<IEnumerable<SyncEventResponseDto>> GetAllAsync();
        Task<SyncEventResponseDto?> GetByIdAsync(Guid syncEventId);
        Task DeleteAsync(Guid syncEventId);
        Task RegisterSyncEventAsync(string eventType, object data);
        Task MarkAsProcessedAsync(Guid syncEventId);
        Task MarkAsFailedAsync(Guid syncEventId, string errorMessage);
    }
}
