using PulseHub.Domain.Entities;
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
                EventType = "StockUpdate",
                Status = "Success",
                Message = "Stock updated successfully.",
                EventDate = DateTime.UtcNow
            };
        }
    }
}
