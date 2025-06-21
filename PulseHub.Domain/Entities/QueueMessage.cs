using System;

namespace PulseHub.Domain.Entities
{
    public class QueueMessage
    {
        public string Channel { get; set; } = string.Empty;
        public Guid QueueMessageId { get; set; } // PK
        public Guid SyncEventId { get; set; } // FK

        public string Payload { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public bool IsProcessed { get; set; } = false;

        public int RetryCount { get; set; } = 0;
        public string? ErrorMessage { get; set; }
        public DateTime? LastAttemptAt { get; set; }

        // Navegação
        public SyncEvent SyncEvent { get; set; }
    }
}
