using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Application.Mappings.Extensions;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Domain.Interfaces;
using PulseHub.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PulseHub.Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessagePublisher _publisher;

        public ProductService(
            IProductRepository productRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IMessagePublisher publisher)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<ProductResponseDto?> GetByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return product is null ? null : _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> CreateAsync(ProductRequestDto productDto)
        {
            var product = productDto.ToEntity();

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<ProductResponseDto>(product);

            await PublishEventAsync("ProductCreated", response);

            return response;
        }

        public async Task<ProductResponseDto> UpdateAsync(Guid productId, ProductRequestDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
                throw new Exception("Produto não encontrado.");

            productDto.UpdateEntity(product);

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<ProductResponseDto>(product);

            await PublishEventAsync("ProductUpdated", response);

            return response;
        }

        public async Task DeleteAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
                throw new Exception("Produto não encontrado.");

            _productRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            await PublishEventAsync("ProductDeleted", new { ProductId = productId });
        }

        private async Task PublishEventAsync(string eventType, object data)
        {
            var message = new IntegrationMessage
            {
                EventType = eventType,
                Data = data
            };

            var json = JsonSerializer.Serialize(message);

            await _publisher.PublishAsync(json);
        }
    }
}
