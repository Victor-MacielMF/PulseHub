using System;

namespace PulseHub.Domain
{
    public class QueueMessage
    {
        public Guid QueueMessageId { get; set; } // PK
        public Guid SyncEventId { get; set; } // FK

        public string Payload { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public bool IsProcessed { get; set; } = true;

        //Navegação
        public SyncEvent? SyncEvent { get; set; }
    }
}
