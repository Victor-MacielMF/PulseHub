using PulseHub.Domain.Entities;
using PulseHub.Domain.Enums;
using System;

namespace PulseHub.Application.Tests.TestHelpers
{
    public static class QueueMessageBuilder
    {
        public static QueueMessage CreateValidQueueMessage(
            Guid? id = null,
            Guid? syncEventId = null,
            string channel = "test-channel",
            string payload = "{\"example\":\"data\"}",
            QueueMessageStatus status = QueueMessageStatus.Pending,
            bool isProcessed = false,
            int retryCount = 0,
            string? errorMessage = null,
            DateTime? publishedAt = null,
            DateTime? lastAttemptAt = null)
        {
            return new QueueMessage
            {
                QueueMessageId = id ?? Guid.NewGuid(),
                SyncEventId = syncEventId ?? Guid.NewGuid(),
                Channel = channel,
                Payload = payload,
                Status = status,
                IsProcessed = isProcessed,
                RetryCount = retryCount,
                ErrorMessage = errorMessage,
                PublishedAt = publishedAt ?? DateTime.UtcNow,
                LastAttemptAt = lastAttemptAt
            };
        }
    }
}
