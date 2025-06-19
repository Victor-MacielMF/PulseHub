using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;

namespace PulseHub.Application.Mappings.Extensions
{
    public static class QueueMessageMappingExtensions
    {
        // 🔄 Entidade (QueueMessage) → DTO (Response)
        public static QueueMessageResponseDto ToResponse(this QueueMessage entity)
        {
            return new QueueMessageResponseDto
            {
                QueueMessageId = entity.QueueMessageId,
                SyncEventId = entity.SyncEventId,
                Payload = entity.Payload,
                PublishedAt = entity.PublishedAt,
                IsProcessed = entity.IsProcessed
            };
        }
    }
}
