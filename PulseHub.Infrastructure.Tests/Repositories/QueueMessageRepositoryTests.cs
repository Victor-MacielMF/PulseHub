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

        [Fact(DisplayName = "Should add and retrieve a queue message successfully")]
        public async Task Should_Add_And_Get_QueueMessage_Successfully()
        {
            // Arrange
            var syncEvent = CreateValidSyncEvent();

            var message = CreateValidQueueMessage(syncEvent.SyncEventId);

            await _context.SyncEvents.AddAsync(syncEvent);
            await _repository.AddAsync(message);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(message.QueueMessageId);

            // Assert
            result.Should().NotBeNull();
            result!.QueueMessageId.Should().Be(message.QueueMessageId);
            result.Payload.Should().Be(message.Payload);
            result.IsProcessed.Should().BeFalse();
        }

        [Fact(DisplayName = "Should retrieve all queue messages")]
        public async Task Should_Get_All_QueueMessages_Successfully()
        {
            // Arrange
            var syncEvent = CreateValidSyncEvent();

            var messages = new List<QueueMessage>
            {
                CreateValidQueueMessage(syncEvent.SyncEventId, "{\"msg\":\"1\"}"),
                CreateValidQueueMessage(syncEvent.SyncEventId, "{\"msg\":\"2\"}", true)
            };

            await _context.SyncEvents.AddAsync(syncEvent);
            foreach (var message in messages)
            {
                await _repository.AddAsync(message);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact(DisplayName = "Should update a queue message successfully")]
        public async Task Should_Update_QueueMessage_Successfully()
        {
            // Arrange
            var syncEvent = CreateValidSyncEvent();

            var message = CreateValidQueueMessage(syncEvent.SyncEventId, "{\"status\":\"pending\"}");

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

        [Fact(DisplayName = "Should delete a queue message successfully")]
        public async Task Should_Delete_QueueMessage_Successfully()
        {
            // Arrange
            var syncEvent = CreateValidSyncEvent();

            var message = CreateValidQueueMessage(syncEvent.SyncEventId, "{\"msg\":\"to delete\"}");

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

        private QueueMessage CreateValidQueueMessage(Guid syncEventId, string? payload = null, bool isProcessed = false)
        {
            return new QueueMessage
            {
                QueueMessageId = Guid.NewGuid(),
                SyncEventId = syncEventId,
                Channel = "MercadoLivre",
                Payload = payload ?? "{\"example\":\"data\"}",
                PublishedAt = DateTime.UtcNow,
                IsProcessed = isProcessed
            };
        }

        private SyncEvent CreateValidSyncEvent()
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
