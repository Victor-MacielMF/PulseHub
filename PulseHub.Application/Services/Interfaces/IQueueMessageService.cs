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
        /// Publica um evento nas filas e registra uma QueueMessage para cada canal.
        /// </summary>
        Task DispatchEventAsync(Guid syncEventId);

        /// <summary>
        /// Atualiza o status da mensagem para processada.
        /// </summary>
        Task MarkAsProcessedAsync(Guid queueMessageId);
    }
}
