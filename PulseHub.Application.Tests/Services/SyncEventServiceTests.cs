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
    public class SyncEventServiceTests
    {
        private readonly Mock<ISyncEventRepository> _syncEventRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly ISyncEventService _syncEventService;

        public SyncEventServiceTests()
        {
            _syncEventRepositoryMock = new Mock<ISyncEventRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new SyncEventProfile());
            });

            _mapper = mapperConfig.CreateMapper();

            _syncEventService = new SyncEventService(
                _syncEventRepositoryMock.Object,
                _mapper,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Should_Get_SyncEvent_By_Id_Successfully()
        {
            // Arrange
            var syncEventId = Guid.NewGuid();
            var syncEvent = SyncEventBuilder.CreateValidSyncEvent(syncEventId);

            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync(syncEvent);

            // Act
            var result = await _syncEventService.GetByIdAsync(syncEventId);

            // Assert
            result.Should().NotBeNull();
            result!.SyncEventId.Should().Be(syncEventId);
            result.EventType.Should().Be(syncEvent.EventType);
        }

        [Fact]
        public async Task Should_Return_Null_When_SyncEvent_Not_Found()
        {
            // Arrange
            var syncEventId = Guid.NewGuid();

            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync((SyncEvent?)null);

            // Act
            var result = await _syncEventService.GetByIdAsync(syncEventId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Get_All_SyncEvents_Successfully()
        {
            // Arrange
            var syncEvents = new List<SyncEvent>
            {
                SyncEventBuilder.CreateValidSyncEvent(),
                SyncEventBuilder.CreateValidSyncEvent()
            };

            _syncEventRepositoryMock.Setup(r => r.GetAllAsync())
                                    .ReturnsAsync(syncEvents);

            // Act
            var result = await _syncEventService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_Delete_SyncEvent_Successfully()
        {
            // Arrange
            var syncEventId = Guid.NewGuid();
            var existingEvent = SyncEventBuilder.CreateValidSyncEvent(syncEventId);

            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync(existingEvent);

            // Act
            await _syncEventService.DeleteAsync(syncEventId);

            // Assert
            _syncEventRepositoryMock.Verify(r => r.Delete(existingEvent), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Delete_SyncEvent_Not_Found()
        {
            // Arrange
            var syncEventId = Guid.NewGuid();
            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync((SyncEvent?)null);

            // Act
            Func<Task> act = async () => await _syncEventService.DeleteAsync(syncEventId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Evento de sincronização não encontrado.");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
