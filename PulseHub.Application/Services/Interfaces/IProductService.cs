using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PulseHub.Application.DTOs;

namespace PulseHub.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync(bool? isActive);
        Task<ProductResponseDto?> GetByIdAsync(Guid productId);
        Task<ProductResponseDto> CreateAsync(ProductRequestDto productDto);
        Task<ProductResponseDto> UpdateAsync(Guid productId, ProductRequestDto productDto);
        Task DeleteAsync(Guid productId);
    }
}
