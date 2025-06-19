using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;

namespace PulseHub.Application.Mappings.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Mapeia de Product para ProductResponseDto
            CreateMap<Product, ProductResponseDto>();

            // Mapeia de ProductResponseDto para Product (se precisar)
            CreateMap<ProductResponseDto, Product>();
        }
    }
}
