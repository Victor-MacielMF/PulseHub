using System;

namespace PulseHub.Domain.Messaging
{
    public class IntegrationMessage
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string EventType { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public object Data { get; set; } = null!;
    }
}
