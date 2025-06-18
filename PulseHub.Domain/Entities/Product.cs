using System;
using System.Collections.Generic;

namespace PulseHub.Domain.Entities
{
    public class Product
    {
        public Guid ProductId { get; set; } //PK
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty ;
        public decimal Price { get; set; } = decimal.Zero ;
        public int Stock {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get;set; } = DateTime.UtcNow ;
        public bool IsActive { get; set; } = true ;

        //Relacionamentos
        public ICollection<SyncEvent> SyncEvents { get; set; } = new List<SyncEvent>();
    }
}
 