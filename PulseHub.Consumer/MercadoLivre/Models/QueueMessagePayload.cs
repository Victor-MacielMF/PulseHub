using System;

namespace PulseHub.Consumer.MercadoLivre.Messaging.Models
{
    public class QueueMessagePayload
    {
        public Guid QueueMessageId { get; set; }
        public Guid EventId { get; set; }
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; }
        public ProductData Data { get; set; }
    }

    public class ProductData
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
