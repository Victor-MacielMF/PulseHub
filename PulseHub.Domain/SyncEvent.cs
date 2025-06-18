using System;
using System.Collections;
using System.Collections.Generic;

namespace PulseHub.Domain
{
    public class SyncEvent
    {
        public Guid SyncEventId { get; set; } // PK
        public Guid ProductId { get; set; } // FK

        public string EventType { get; set; } = string.Empty;
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }

        //Relacionamento
        public ICollection<QueueMessage> QueueMessage { get; set; } = new List<QueueMessage>();

        //Navegação
        public Product? Product { get; set; }
    }
}
