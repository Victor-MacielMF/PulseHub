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



        // 🔥 Métodos de domínio
        public void SetAsProcessed()
        {
            Status = QueueMessageStatus.Processed;
            IsProcessed = true;
            LastAttemptAt = DateTime.UtcNow;
            ErrorMessage = null;
        }

        public void SetAsFailed(string errorMessage)
        {
            Status = QueueMessageStatus.Failed;
            IsProcessed = false;
            RetryCount++;
            LastAttemptAt = DateTime.UtcNow;
            ErrorMessage = errorMessage;
        }

        public void SetAsEnqueued()
        {
            Status = QueueMessageStatus.Enqueued;
            IsProcessed = false;
            PublishedAt = DateTime.UtcNow;
        }

        public void SetAsPending()
        {
            Status = QueueMessageStatus.Pending;
            IsProcessed = false;
            LastAttemptAt = DateTime.UtcNow;
            ErrorMessage = null;
        }
    }
}
