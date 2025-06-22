using System;
using System.Collections.Generic;

namespace PulseHub.Domain.Entities
{
    public class SyncEvent
    {
        public Guid SyncEventId { get; set; } // Primary Key

        public Guid ProductId { get; set; } // Foreign Key

        public string EventType { get; set; } = string.Empty;

        public DateTime EventDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Failed

        public string Message { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<QueueMessage> QueueMessages { get; set; } = new List<QueueMessage>();

        public Product Product { get; set; } = null!;
    }
}
