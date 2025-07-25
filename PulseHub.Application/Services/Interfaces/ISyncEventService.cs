﻿using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;
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
        Task<SyncEvent> RegisterSyncEventAsync(string eventType, object data);
        Task PublishToIntegrationAsync(Guid syncEventId);
        Task MarkAsProcessedAsync(Guid syncEventId);
    }
}
