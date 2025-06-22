using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Domain.Entities;

namespace PulseHub.Application.Mappings.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Maps Product entity to ProductResponseDto
            CreateMap<Product, ProductResponseDto>();

            // Maps ProductResponseDto to Product entity (if needed)
            CreateMap<ProductResponseDto, Product>();
        }
    }
}
