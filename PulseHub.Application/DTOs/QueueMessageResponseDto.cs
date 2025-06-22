using System;

namespace PulseHub.Application.DTOs
{
    public class QueueMessageResponseDto
    {
        public Guid QueueMessageId { get; set; }
        public Guid SyncEventId { get; set; }

        public string Channel { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;

        public DateTime PublishedAt { get; set; }
        public bool IsProcessed { get; set; }

        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? LastAttemptAt { get; set; }

    }
}
