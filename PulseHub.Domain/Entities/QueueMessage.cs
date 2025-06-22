using PulseHub.Domain.Enums;
using System;

namespace PulseHub.Domain.Entities
{
    public class QueueMessage
    {
        public Guid QueueMessageId { get; set; } // Primary Key

        public Guid SyncEventId { get; set; } // Foreign Key

        public string Channel { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        public bool IsProcessed { get; set; } = false;

        public QueueMessageStatus Status { get; set; } = QueueMessageStatus.Pending;

        public int RetryCount { get; set; } = 0;

        public string? ErrorMessage { get; set; }

        public DateTime? LastAttemptAt { get; set; }

        // Navigation property
        public SyncEvent SyncEvent { get; set; }
    }
}
