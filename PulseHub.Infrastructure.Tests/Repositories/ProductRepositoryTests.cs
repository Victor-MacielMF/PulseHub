using FluentAssertions;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Interfaces;
using PulseHub.Infrastructure.Data;
using PulseHub.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PulseHub.Infrastructure.Tests.Repositories
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly PulseHubDbContext _context;
        private readonly IProductRepository _repository;

        public ProductRepositoryTests()
        {
            _context = TestDbContextFactory.CreateDbContext();
            _repository = new ProductRepository(_context);
        }

        [Fact(DisplayName = "Should add and retrieve a product successfully")]
        public async Task Should_Add_And_Get_Product_Successfully()
        {
            // Arrange
            var product = CreateValidProduct();

            // Act
            await _repository.AddAsync(product);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(product.ProductId);

            // Assert
            result.Should().NotBeNull();
            result!.ProductId.Should().Be(product.ProductId);
            result.Name.Should().Be(product.Name);
            result.Description.Should().Be(product.Description);
            result.Price.Should().Be(product.Price);
            result.Stock.Should().Be(product.Stock);
            result.IsActive.Should().BeTrue();
        }

        [Fact(DisplayName = "Should retrieve all products successfully")]
        public async Task Should_Get_All_Products_Successfully()
        {
            // Arrange
            var products = new List<Product>
            {
                CreateValidProduct(),
                CreateValidProduct()
            };

            foreach (var product in products)
            {
                await _repository.AddAsync(product);
            }

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(isActive: true);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact(DisplayName = "Should update a product successfully")]
        public async Task Should_Update_Product_Successfully()
        {
            // Arrange
            var product = CreateValidProduct();

            await _repository.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            product.Name = "Updated Name";
            product.Description = "Updated Description";

            _repository.Update(product);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(product.ProductId);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Updated Name");
            result.Description.Should().Be("Updated Description");
        }

        [Fact(DisplayName = "Should soft delete a product successfully")]
        public async Task Should_Soft_Delete_Product_Successfully()
        {
            // Arrange
            var product = CreateValidProduct();

            await _repository.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            _repository.Delete(product);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(product.ProductId);

            // Assert
            result.Should().NotBeNull();
            result!.IsActive.Should().BeFalse();
        }

        private Product CreateValidProduct()
        {
            return new Product
            {
                ProductId = Guid.NewGuid(),
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                Stock = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
