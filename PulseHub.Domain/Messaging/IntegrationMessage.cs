using System;

namespace PulseHub.Domain.Messaging
{
    public class IntegrationMessage<T>
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string EventType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public T Data { get; set; } = default!;
    }
}
