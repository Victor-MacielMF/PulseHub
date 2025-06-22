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

        [Fact(DisplayName = "Should add and retrieve a sync event successfully")]
        public async Task Should_Add_And_Get_SyncEvent_Successfully()
        {
            // Arrange
            var product = CreateValidProduct();
            var syncEvent = CreateValidSyncEvent(product.ProductId);

            await _context.Products.AddAsync(product);
            await _repository.AddAsync(syncEvent);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(syncEvent.SyncEventId);

            // Assert
            result.Should().NotBeNull();
            result!.EventType.Should().Be(syncEvent.EventType);
            result.Status.Should().Be(syncEvent.Status);
            result.Message.Should().Be(syncEvent.Message);
        }

        [Fact(DisplayName = "Should retrieve all sync events")]
        public async Task Should_Get_All_SyncEvents_Successfully()
        {
            // Arrange
            var product = CreateValidProduct();

            var events = new List<SyncEvent>
            {
                CreateValidSyncEvent(product.ProductId, "Event1", "Message1", "Pending"),
                CreateValidSyncEvent(product.ProductId, "Event2", "Message2", "Completed")
            };

            await _context.Products.AddAsync(product);
            foreach (var ev in events)
            {
                await _repository.AddAsync(ev);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact(DisplayName = "Should update a sync event successfully")]
        public async Task Should_Update_SyncEvent_Successfully()
        {
            // Arrange
            var product = CreateValidProduct();
            var syncEvent = CreateValidSyncEvent(product.ProductId, "EventBefore", "Before update", "Pending");

            await _context.Products.AddAsync(product);
            await _repository.AddAsync(syncEvent);
            await _context.SaveChangesAsync();

            // Act
            syncEvent.EventType = "EventAfter";
            syncEvent.Status = "Completed";
            syncEvent.Message = "After update";

            _repository.Update(syncEvent);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(syncEvent.SyncEventId);

            // Assert
            result.Should().NotBeNull();
            result!.EventType.Should().Be("EventAfter");
            result.Status.Should().Be("Completed");
            result.Message.Should().Be("After update");
        }

        [Fact(DisplayName = "Should delete a sync event successfully")]
        public async Task Should_Delete_SyncEvent_Successfully()
        {
            // Arrange
            var product = CreateValidProduct();
            var syncEvent = CreateValidSyncEvent(product.ProductId, "EventToDelete", "Delete me", "Pending");

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

        // 🔧 Helpers

        private SyncEvent CreateValidSyncEvent(Guid productId, string eventType = "StockUpdate", string message = "Test sync event", string status = "Pending")
        {
            return new SyncEvent
            {
                SyncEventId = Guid.NewGuid(),
                ProductId = productId,
                EventType = eventType,
                EventDate = DateTime.UtcNow,
                Status = status,
                Message = message
            };
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
