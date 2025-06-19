using PulseHub.Domain.Entities;
using System;

namespace PulseHub.Application.Tests.TestHelpers
{
    public static class ProductBuilder
    {
        public static Product CreateValidProduct(Guid? id = null)
        {
            return new Product
            {
                ProductId = id ?? Guid.NewGuid(),
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                Stock = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
