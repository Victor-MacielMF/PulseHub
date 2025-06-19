using PulseHub.Domain.Entities;
using System;

namespace PulseHub.Application.Tests.TestHelpers
{
    public static class QueueMessageBuilder
    {
        public static QueueMessage CreateValidQueueMessage(Guid? id = null, Guid? syncEventId = null)
        {
            return new QueueMessage
            {
                QueueMessageId = id ?? Guid.NewGuid(),
                SyncEventId = syncEventId ?? Guid.NewGuid(),
                Payload = "{\"example\":\"data\"}",
                PublishedAt = DateTime.UtcNow,
                IsProcessed = false
            };
        }
    }
}
