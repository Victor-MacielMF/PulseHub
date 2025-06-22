using AutoMapper;
using FluentAssertions;
using Moq;
using PulseHub.Application.DTOs;
using PulseHub.Application.Mappings.Profiles;
using PulseHub.Application.Services.Implementations;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Application.Tests.TestHelpers;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Enums;
using PulseHub.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PulseHub.Application.Tests.Services
{
    public class SyncEventServiceTests
    {
        private readonly Mock<ISyncEventRepository> _syncEventRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IQueueMessageService> _queueMessageServiceMock;
        private readonly IMapper _mapper;
        private readonly ISyncEventService _syncEventService;

        public SyncEventServiceTests()
        {
            _syncEventRepositoryMock = new Mock<ISyncEventRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _queueMessageServiceMock = new Mock<IQueueMessageService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new SyncEventProfile());
            });

            _mapper = mapperConfig.CreateMapper();

            _syncEventService = new SyncEventService(
                _syncEventRepositoryMock.Object,
                _queueMessageServiceMock.Object,
                _unitOfWorkMock.Object,
                _mapper
            );
        }

        [Fact]
        public async Task Should_Get_SyncEvent_By_Id_Successfully()
        {
            var syncEventId = Guid.NewGuid();
            var syncEvent = SyncEventBuilder.CreateValidSyncEvent(syncEventId);

            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync(syncEvent);

            var result = await _syncEventService.GetByIdAsync(syncEventId);

            result.Should().NotBeNull();
            result!.SyncEventId.Should().Be(syncEventId);
            result.EventType.Should().Be(syncEvent.EventType);
        }

        [Fact]
        public async Task Should_Return_Null_When_SyncEvent_Not_Found()
        {
            var syncEventId = Guid.NewGuid();

            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync((SyncEvent?)null);

            var result = await _syncEventService.GetByIdAsync(syncEventId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Get_All_SyncEvents_Successfully()
        {
            var syncEvents = new List<SyncEvent>
            {
                SyncEventBuilder.CreateValidSyncEvent(),
                SyncEventBuilder.CreateValidSyncEvent()
            };

            _syncEventRepositoryMock.Setup(r => r.GetAllAsync())
                                    .ReturnsAsync(syncEvents);

            var result = await _syncEventService.GetAllAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_Delete_SyncEvent_Successfully()
        {
            var syncEventId = Guid.NewGuid();
            var existingEvent = SyncEventBuilder.CreateValidSyncEvent(syncEventId);

            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync(existingEvent);

            await _syncEventService.DeleteAsync(syncEventId);

            _syncEventRepositoryMock.Verify(r => r.Delete(existingEvent), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Delete_SyncEvent_Not_Found()
        {
            var syncEventId = Guid.NewGuid();
            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync((SyncEvent?)null);

            Func<Task> act = async () => await _syncEventService.DeleteAsync(syncEventId);

            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Sync event not found.");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Should_Register_SyncEvent_Successfully()
        {
            var data = new { ProductId = Guid.NewGuid(), Name = "Test" };

            var result = await _syncEventService.RegisterSyncEventAsync("ProductCreated", data);

            result.Should().NotBeNull();
            result.EventType.Should().Be("ProductCreated");
            result.Payload.Should().Contain("ProductId");

            _syncEventRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SyncEvent>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Mark_SyncEvent_As_Processed()
        {
            var syncEventId = Guid.NewGuid();
            var syncEvent = SyncEventBuilder.CreateValidSyncEvent(syncEventId);
            syncEvent.Status = SyncEventStatus.Pending;

            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync(syncEvent);

            await _syncEventService.MarkAsProcessedAsync(syncEventId);

            syncEvent.Status.Should().Be(SyncEventStatus.Completed);
            _syncEventRepositoryMock.Verify(r => r.Update(syncEvent), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_MarkAsProcessed_Not_Found()
        {
            var syncEventId = Guid.NewGuid();

            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync((SyncEvent?)null);

            Func<Task> act = async () => await _syncEventService.MarkAsProcessedAsync(syncEventId);

            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Sync event not found.");

            _syncEventRepositoryMock.Verify(r => r.Update(It.IsAny<SyncEvent>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Should_Publish_To_Integration_Successfully()
        {
            var syncEventId = Guid.NewGuid();

            await _syncEventService.PublishToIntegrationAsync(syncEventId);

            _queueMessageServiceMock.Verify(q => q.DispatchEventAsync(syncEventId), Times.Once);
        }
    }
}
