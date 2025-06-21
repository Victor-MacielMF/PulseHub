using PulseHub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseHub.Application.Services.Interfaces
{
    public interface IQueueMessageService
    {
        Task<IEnumerable<QueueMessageResponseDto>> GetAllAsync();
        Task<QueueMessageResponseDto?> GetByIdAsync(Guid queueMessageId);
        Task DeleteAsync(Guid queueMessageId);
        Task RegisterQueueMessageAsync(Guid syncEventId, string payload, string channel);
        Task MarkAsProcessedAsync(Guid queueMessageId);
    }
}
