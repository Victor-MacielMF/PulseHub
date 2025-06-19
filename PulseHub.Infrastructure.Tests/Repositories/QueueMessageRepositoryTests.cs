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
    public class QueueMessageRepositoryTests : IDisposable
    {
        private readonly PulseHubDbContext _context;
        private readonly IQueueMessageRepository _repository;

        public QueueMessageRepositoryTests()
        {
            _context = TestDbContextFactory.CreateDbContext();
            _repository = new QueueMessageRepository(_context);
        }

        [Fact]
        public async Task Should_Add_And_Get_QueueMessage_Successfully()
        {
            // Arrange
            var syncEvent = CreateFakeSyncEvent();

            var message = new QueueMessage
            {
                QueueMessageId = Guid.NewGuid(),
                SyncEventId = syncEvent.SyncEventId,
                Payload = "{\"teste\":\"data\"}",
                PublishedAt = DateTime.UtcNow,
                IsProcessed = false
            };

            await _context.SyncEvents.AddAsync(syncEvent);
            await _repository.AddAsync(message);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(message.QueueMessageId);

            // Assert
            result.Should().NotBeNull();
            result!.Payload.Should().Be("{\"teste\":\"data\"}");
            result.IsProcessed.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Get_All_QueueMessages()
        {
            // Arrange
            var syncEvent = CreateFakeSyncEvent();

            var messages = new List<QueueMessage>
            {
                new QueueMessage
                {
                    QueueMessageId = Guid.NewGuid(),
                    SyncEventId = syncEvent.SyncEventId,
                    Payload = "{\"msg\":\"1\"}",
                    PublishedAt = DateTime.UtcNow,
                    IsProcessed = false
                },
                new QueueMessage
                {
                    QueueMessageId = Guid.NewGuid(),
                    SyncEventId = syncEvent.SyncEventId,
                    Payload = "{\"msg\":\"2\"}",
                    PublishedAt = DateTime.UtcNow,
                    IsProcessed = true
                }
            };

            await _context.SyncEvents.AddAsync(syncEvent);
            await _repository.AddAsync(messages[0]);
            await _repository.AddAsync(messages[1]);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_Update_QueueMessage_Successfully()
        {
            // Arrange
            var syncEvent = CreateFakeSyncEvent();

            var message = new QueueMessage
            {
                QueueMessageId = Guid.NewGuid(),
                SyncEventId = syncEvent.SyncEventId,
                Payload = "{\"status\":\"pending\"}",
                PublishedAt = DateTime.UtcNow,
                IsProcessed = false
            };

            await _context.SyncEvents.AddAsync(syncEvent);
            await _repository.AddAsync(message);
            await _context.SaveChangesAsync();

            // Act
            message.Payload = "{\"status\":\"processed\"}";
            message.IsProcessed = true;
            _repository.Update(message);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(message.QueueMessageId);

            // Assert
            result.Should().NotBeNull();
            result!.Payload.Should().Be("{\"status\":\"processed\"}");
            result.IsProcessed.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Delete_QueueMessage_Successfully()
        {
            // Arrange
            var syncEvent = CreateFakeSyncEvent();

            var message = new QueueMessage
            {
                QueueMessageId = Guid.NewGuid(),
                SyncEventId = syncEvent.SyncEventId,
                Payload = "{\"msg\":\"to delete\"}",
                PublishedAt = DateTime.UtcNow,
                IsProcessed = false
            };

            await _context.SyncEvents.AddAsync(syncEvent);
            await _repository.AddAsync(message);
            await _context.SaveChangesAsync();

            // Act
            _repository.Delete(message);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(message.QueueMessageId);

            // Assert
            result.Should().BeNull();
        }

        private SyncEvent CreateFakeSyncEvent()
        {
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = "Product for SyncEvent",
                Description = "Description",
                Price = 100,
                Stock = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return new SyncEvent
            {
                SyncEventId = Guid.NewGuid(),
                ProductId = product.ProductId,
                EventType = "TestEvent",
                EventDate = DateTime.UtcNow,
                Status = "Pending",
                Message = "Test message"
            };
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
