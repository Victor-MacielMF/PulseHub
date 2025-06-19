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
    public class QueueMessageServiceTests
    {
        private readonly Mock<IQueueMessageRepository> _queueMessageRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly IQueueMessageService _queueMessageService;

        public QueueMessageServiceTests()
        {
            _queueMessageRepositoryMock = new Mock<IQueueMessageRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new QueueMessageProfile());
            });

            _mapper = mapperConfig.CreateMapper();

            _queueMessageService = new QueueMessageService(
                _queueMessageRepositoryMock.Object,
                _mapper,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Should_Get_QueueMessage_By_Id_Successfully()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var message = QueueMessageBuilder.CreateValidQueueMessage(messageId);

            _queueMessageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                                       .ReturnsAsync(message);

            // Act
            var result = await _queueMessageService.GetByIdAsync(messageId);

            // Assert
            result.Should().NotBeNull();
            result!.QueueMessageId.Should().Be(messageId);
            result.Payload.Should().Be(message.Payload);
        }

        [Fact]
        public async Task Should_Return_Null_When_QueueMessage_Not_Found()
        {
            // Arrange
            var messageId = Guid.NewGuid();

            _queueMessageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                                       .ReturnsAsync((QueueMessage?)null);

            // Act
            var result = await _queueMessageService.GetByIdAsync(messageId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Get_All_QueueMessages_Successfully()
        {
            // Arrange
            var messages = new List<QueueMessage>
            {
                QueueMessageBuilder.CreateValidQueueMessage(),
                QueueMessageBuilder.CreateValidQueueMessage()
            };

            _queueMessageRepositoryMock.Setup(r => r.GetAllAsync())
                                       .ReturnsAsync(messages);

            // Act
            var result = await _queueMessageService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_Delete_QueueMessage_Successfully()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var existingMessage = QueueMessageBuilder.CreateValidQueueMessage(messageId);

            _queueMessageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                                       .ReturnsAsync(existingMessage);

            // Act
            await _queueMessageService.DeleteAsync(messageId);

            // Assert
            _queueMessageRepositoryMock.Verify(r => r.Delete(existingMessage), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Delete_QueueMessage_Not_Found()
        {
            // Arrange
            var messageId = Guid.NewGuid();

            _queueMessageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                                       .ReturnsAsync((QueueMessage?)null);

            // Act
            Func<Task> act = async () => await _queueMessageService.DeleteAsync(messageId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Mensagem não encontrada.");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
