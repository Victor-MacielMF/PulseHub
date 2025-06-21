using PulseHub.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseHub.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync(bool? isActive);
        Task AddAsync(Product product);
        void Update(Product product);
        void Delete(Product product);
    }
}
