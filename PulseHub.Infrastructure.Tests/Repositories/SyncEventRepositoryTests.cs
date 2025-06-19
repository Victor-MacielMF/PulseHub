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
    public class SyncEventRepositoryTests : IDisposable
    {
        private readonly PulseHubDbContext _context;
        private readonly ISyncEventRepository _repository;

        public SyncEventRepositoryTests()
        {
            _context = TestDbContextFactory.CreateDbContext();
            _repository = new SyncEventRepository(_context);
        }

        [Fact]
        public async Task Should_Add_And_Get_SyncEvent_Successfully()
        {
            // Arrange
            var product = CreateFakeProduct();

            var syncEvent = new SyncEvent
            {
                SyncEventId = Guid.NewGuid(),
                ProductId = product.ProductId,
                EventType = "StockUpdate",
                EventDate = DateTime.UtcNow,
                Status = "Pending",
                Message = "Test sync event"
            };

            await _context.Products.AddAsync(product);
            await _repository.AddAsync(syncEvent);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(syncEvent.SyncEventId);

            // Assert
            result.Should().NotBeNull();
            result!.EventType.Should().Be("StockUpdate");
            result.Status.Should().Be("Pending");
            result.Message.Should().Be("Test sync event");
        }

        [Fact]
        public async Task Should_Get_All_SyncEvents()
        {
            // Arrange
            var product = CreateFakeProduct();

            var events = new List<SyncEvent>
            {
                new SyncEvent
                {
                    SyncEventId = Guid.NewGuid(),
                    ProductId = product.ProductId,
                    EventType = "Event1",
                    EventDate = DateTime.UtcNow,
                    Status = "Pending",
                    Message = "Message 1"
                },
                new SyncEvent
                {
                    SyncEventId = Guid.NewGuid(),
                    ProductId = product.ProductId,
                    EventType = "Event2",
                    EventDate = DateTime.UtcNow,
                    Status = "Completed",
                    Message = "Message 2"
                }
            };

            await _context.Products.AddAsync(product);
            await _repository.AddAsync(events[0]);
            await _repository.AddAsync(events[1]);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_Update_SyncEvent_Successfully()
        {
            // Arrange
            var product = CreateFakeProduct();

            var syncEvent = new SyncEvent
            {
                SyncEventId = Guid.NewGuid(),
                ProductId = product.ProductId,
                EventType = "EventBeforeUpdate",
                EventDate = DateTime.UtcNow,
                Status = "Pending",
                Message = "Before update"
            };

            await _context.Products.AddAsync(product);
            await _repository.AddAsync(syncEvent);
            await _context.SaveChangesAsync();

            // Act
            syncEvent.EventType = "EventAfterUpdate";
            syncEvent.Status = "Completed";
            syncEvent.Message = "After update";

            _repository.Update(syncEvent);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(syncEvent.SyncEventId);

            // Assert
            result.Should().NotBeNull();
            result!.EventType.Should().Be("EventAfterUpdate");
            result.Status.Should().Be("Completed");
            result.Message.Should().Be("After update");
        }

        [Fact]
        public async Task Should_Delete_SyncEvent_Successfully()
        {
            // Arrange
            var product = CreateFakeProduct();

            var syncEvent = new SyncEvent
            {
                SyncEventId = Guid.NewGuid(),
                ProductId = product.ProductId,
                EventType = "EventToDelete",
                EventDate = DateTime.UtcNow,
                Status = "Pending",
                Message = "Delete me"
            };

            await _context.Products.AddAsync(product);
            await _repository.AddAsync(syncEvent);
            await _context.SaveChangesAsync();

            // Act
            _repository.Delete(syncEvent);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(syncEvent.SyncEventId);

            // Assert
            result.Should().BeNull();
        }

        private Product CreateFakeProduct()
        {
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = "Product for SyncEvent",
                Description = "Test Description",
                Price = 100,
                Stock = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            return product;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
