using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;

namespace PulseHub.Application.Mappings.Extensions
{
    public static class QueueMessageMappingExtensions
    {
        public static QueueMessageResponseDto ToResponse(this QueueMessage entity)
        {
            return new QueueMessageResponseDto
            {
                QueueMessageId = entity.QueueMessageId,
                SyncEventId = entity.SyncEventId,
                Payload = entity.Payload,
                PublishedAt = entity.PublishedAt,
                IsProcessed = entity.IsProcessed,
                RetryCount = entity.RetryCount,
                ErrorMessage = entity.ErrorMessage,
                LastAttemptAt = entity.LastAttemptAt,
                Channel = entity.Channel
            };
        }
    }
}
