using System;

namespace PulseHub.Application.DTOs
{
    public class QueueMessageResponseDto
    {
        public string Channel { get; set; }
        public Guid QueueMessageId { get; set; }
        public Guid SyncEventId { get; set; }
        public string Payload { get; set; } = null!;
        public DateTime PublishedAt { get; set; }
        public bool IsProcessed { get; set; }

    }
}
