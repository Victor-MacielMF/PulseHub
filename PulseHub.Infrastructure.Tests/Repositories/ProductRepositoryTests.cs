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

            //SUT
            _repository = new ProductRepository(_context); // Instancia ainda concreta, mas exposta como interface
        }


        [Fact]
        public async Task Should_Add_And_Get_Product_Successfully()
        {
            //Arrange
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                Stock = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            //Act
            await _repository.AddAsync(product);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(product.ProductId);

            //Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Test Product");
            result.Description.Should().Be("Test Description");
        }

        [Fact]
        public async Task Should_Get_All_Products()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product
                {
                    ProductId = Guid.NewGuid(),
                    Name = "Product 1",
                    Description = "Description 1",
                    Price = 50,
                    Stock = 5,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Product
                {
                    ProductId = Guid.NewGuid(),
                    Name = "Product 2",
                    Description = "Description 2",
                    Price = 70,
                    Stock = 7,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            await _repository.AddAsync(products[0]);
            await _repository.AddAsync(products[1]);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_Update_Product_Successfully()
        {
            // Arrange
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = "Product Before Update",
                Description = "Description Before",
                Price = 100,
                Stock = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _repository.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            product.Name = "Product After Update";
            product.Description = "Description After";
            _repository.Update(product);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(product.ProductId);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Product After Update");
            result.Description.Should().Be("Description After");
        }

        [Fact]
        public async Task Should_Delete_Product_Successfully()
        {
            // Arrange
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = "Product to Delete",
                Description = "Description",
                Price = 100,
                Stock = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _repository.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            _repository.Delete(product);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(product.ProductId);

            // Assert
            result.Should().BeNull();
        }


        public void Dispose()
        {
            // Cleanup do banco após cada teste
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
