using AutoMapper;
using FluentAssertions;
using Moq;
using PulseHub.Application.DTOs;
using PulseHub.Application.Mappings.Profiles;
using PulseHub.Application.Services.Implementations;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Application.Tests.TestHelpers;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PulseHub.Application.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly Mock<ISyncEventService> _syncEventServiceMock;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _syncEventServiceMock = new Mock<ISyncEventService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ProductProfile());
            });

            _mapper = mapperConfig.CreateMapper();

            _productService = new ProductService(
                _productRepositoryMock.Object,
                _mapper,
                _unitOfWorkMock.Object,
                _syncEventServiceMock.Object
            );
        }

        [Fact]
        public async Task Should_Get_Product_By_Id_Successfully()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = ProductBuilder.CreateValidProduct(productId);

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                  .ReturnsAsync(product);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            result.Should().NotBeNull();
            result!.ProductId.Should().Be(productId);
            result.Name.Should().Be("Test Product");
            result.Description.Should().Be("Test Description");
        }

        [Fact]
        public async Task Should_Return_Null_When_Product_Not_Found()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                  .ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Create_Product_Successfully()
        {
            // Arrange
            var request = new ProductRequestDto
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                Stock = 10
            };

            // Act
            var result = await _productService.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Test Product");
            result.Description.Should().Be("Test Description");

            _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Update_Product_Successfully()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = ProductBuilder.CreateValidProduct(productId);

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                  .ReturnsAsync(existingProduct);

            var request = new ProductRequestDto
            {
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 200,
                Stock = 20
            };

            // Act
            var result = await _productService.UpdateAsync(productId, request);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Updated Product");
            result.Description.Should().Be("Updated Description");

            _productRepositoryMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Update_Product_Not_Found()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                  .ReturnsAsync((Product?)null);

            var request = new ProductRequestDto
            {
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 200,
                Stock = 20
            };

            // Act
            Func<Task> act = async () => await _productService.UpdateAsync(productId, request);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Product not found.");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Should_Delete_Product_Successfully()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = ProductBuilder.CreateValidProduct(productId);

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                  .ReturnsAsync(existingProduct);

            // Act
            await _productService.DeleteAsync(productId);

            // Assert
            _productRepositoryMock.Verify(r => r.Update(existingProduct), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Delete_Product_Not_Found()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                  .ReturnsAsync((Product?)null);

            // Act
            Func<Task> act = async () => await _productService.DeleteAsync(productId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Product not found.");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
