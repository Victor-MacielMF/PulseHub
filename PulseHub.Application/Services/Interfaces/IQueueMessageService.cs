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

        /// <summary>
        /// Publishes a sync event to the queues and registers a QueueMessage for each channel.
        /// </summary>
        Task DispatchEventAsync(Guid syncEventId);

        /// <summary>
        /// Updates the message status to processed.
        /// </summary>
        Task MarkAsProcessedAsync(Guid queueMessageId);
    }
}
