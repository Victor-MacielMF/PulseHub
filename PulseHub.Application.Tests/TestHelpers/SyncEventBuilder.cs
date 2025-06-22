using PulseHub.Domain.Entities;
using PulseHub.Domain.Enums;
using System;

namespace PulseHub.Application.Tests.TestHelpers
{
    public static class SyncEventBuilder
    {
        public static SyncEvent CreateValidSyncEvent(Guid? id = null, Guid? productId = null)
        {
            return new SyncEvent
            {
                SyncEventId = id ?? Guid.NewGuid(),
                ProductId = productId ?? Guid.NewGuid(),
                EventType = "ProductCreated",
                Status = SyncEventStatus.Pending,
                Message = "Event registered successfully.",
                EventDate = DateTime.UtcNow,
                Payload = "{\"productId\":\"" + (productId ?? Guid.NewGuid()) + "\"}"
            };
        }
    }
}