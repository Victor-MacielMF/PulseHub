using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;
using System;

namespace PulseHub.Application.Mappings.Extensions
{
    public static class ProductMappingExtensions
    {
        // Maps ProductRequestDto to Product entity
        public static Product ToEntity(this ProductRequestDto dto)
        {
            return new Product
            {
                ProductId = Guid.NewGuid(), // Generated during mapping
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        // Maps Product entity to ProductResponseDto
        public static ProductResponseDto ToResponse(this Product entity)
        {
            return new ProductResponseDto
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Stock = entity.Stock,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                IsActive = entity.IsActive
            };
        }

        // Updates an existing Product entity with data from ProductRequestDto
        public static void UpdateEntity(this ProductRequestDto dto, Product entity)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.Stock = dto.Stock;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
