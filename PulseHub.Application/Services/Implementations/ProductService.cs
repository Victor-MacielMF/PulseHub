using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Application.Mappings.Extensions;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseHub.Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISyncEventService _syncEventService;

        public ProductService(
            IProductRepository productRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ISyncEventService syncEventService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _syncEventService = syncEventService;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(bool? isActive)
        {
            var products = await _productRepository.GetAllAsync(isActive);
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

            var syncEvent = await _syncEventService.RegisterSyncEventAsync("ProductCreated", response);
            await _syncEventService.PublishToIntegrationAsync(syncEvent.SyncEventId);

            return response;
        }

        public async Task<ProductResponseDto> UpdateAsync(Guid productId, ProductRequestDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(productId)
                          ?? throw new Exception("Produto não encontrado.");

            productDto.UpdateEntity(product);

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<ProductResponseDto>(product);

            var syncEvent = await _syncEventService.RegisterSyncEventAsync("ProductUpdated", response);
            await _syncEventService.PublishToIntegrationAsync(syncEvent.SyncEventId);

            return response;
        }

        public async Task DeleteAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId)
                          ?? throw new Exception("Produto não encontrado.");

            product.IsActive = false;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            var syncEvent = await _syncEventService.RegisterSyncEventAsync("ProductDeleted", new { ProductId = productId });
            await _syncEventService.PublishToIntegrationAsync(syncEvent.SyncEventId);
        }
    }
}
