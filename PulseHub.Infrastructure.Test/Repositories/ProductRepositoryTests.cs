using FluentAssertions;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Interfaces;
using PulseHub.Infrastructure.Data;
using PulseHub.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PulseHub.Infrastructure.Test.Repositories
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


        public void Dispose()
        {
            // Cleanup do banco após cada teste
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
