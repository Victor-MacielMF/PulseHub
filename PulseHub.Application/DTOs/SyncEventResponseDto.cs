using System;

namespace PulseHub.Application.DTOs
{
    public class SyncEventResponseDto
    {
        public Guid SyncEventId { get; set; }
        public Guid ProductId { get; set; }
        public string EventType { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public string Status { get; set; } = null!;
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; }
    }
}
