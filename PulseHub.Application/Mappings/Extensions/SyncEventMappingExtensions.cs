using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;

namespace PulseHub.Application.Mappings.Extensions
{
    public static class SyncEventMappingExtensions
    {
        public static SyncEventResponseDto ToResponse(this SyncEvent entity)
        {
            return new SyncEventResponseDto
            {
                SyncEventId = entity.SyncEventId,
                ProductId = entity.ProductId,
                EventType = entity.EventType,
                EventDate = entity.EventDate,
                Status = entity.Status,
                Message = entity.Message,
                Payload = entity.Payload
            };
        }
    }
}
