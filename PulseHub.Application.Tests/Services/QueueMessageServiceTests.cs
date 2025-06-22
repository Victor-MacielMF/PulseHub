using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using PulseHub.Application.DTOs;
using PulseHub.Application.Mappings.Profiles;
using PulseHub.Application.Services.Implementations;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Application.Tests.TestHelpers;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Interfaces;
using PulseHub.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PulseHub.Application.Tests.Services
{
    public class QueueMessageServiceTests
    {
        private readonly Mock<IQueueMessageRepository> _queueMessageRepositoryMock;
        private readonly Mock<ISyncEventRepository> _syncEventRepositoryMock;
        private readonly Mock<IMessagePublisher> _publisherMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly IQueueMessageService _queueMessageService;

        public QueueMessageServiceTests()
        {
            _queueMessageRepositoryMock = new Mock<IQueueMessageRepository>();
            _syncEventRepositoryMock = new Mock<ISyncEventRepository>();
            _publisherMock = new Mock<IMessagePublisher>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new QueueMessageProfile());
            });

            _mapper = mapperConfig.CreateMapper();

            var configuration = CreateConfigurationWithChannels();

            _queueMessageService = new QueueMessageService(
                _queueMessageRepositoryMock.Object,
                _syncEventRepositoryMock.Object,
                _publisherMock.Object,
                _mapper,
                _unitOfWorkMock.Object,
                configuration
            );
        }

        private IConfiguration CreateConfigurationWithChannels()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"IntegrationChannels:0", "channel1"},
                {"IntegrationChannels:1", "channel2"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task Should_Get_QueueMessage_By_Id_Successfully()
        {
            var messageId = Guid.NewGuid();
            var message = QueueMessageBuilder.CreateValidQueueMessage(messageId);

            _queueMessageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                                       .ReturnsAsync(message);

            var result = await _queueMessageService.GetByIdAsync(messageId);

            result.Should().NotBeNull();
            result!.QueueMessageId.Should().Be(messageId);
            result.Payload.Should().Be(message.Payload);
        }

        [Fact]
        public async Task Should_Return_Null_When_QueueMessage_Not_Found()
        {
            var messageId = Guid.NewGuid();

            _queueMessageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                                       .ReturnsAsync((QueueMessage?)null);

            var result = await _queueMessageService.GetByIdAsync(messageId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Get_All_QueueMessages_Successfully()
        {
            var messages = new List<QueueMessage>
            {
                QueueMessageBuilder.CreateValidQueueMessage(),
                QueueMessageBuilder.CreateValidQueueMessage()
            };

            _queueMessageRepositoryMock.Setup(r => r.GetAllAsync())
                                       .ReturnsAsync(messages);

            var result = await _queueMessageService.GetAllAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_Delete_QueueMessage_Successfully()
        {
            var messageId = Guid.NewGuid();
            var existingMessage = QueueMessageBuilder.CreateValidQueueMessage(messageId);

            _queueMessageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                                       .ReturnsAsync(existingMessage);

            await _queueMessageService.DeleteAsync(messageId);

            _queueMessageRepositoryMock.Verify(r => r.Delete(existingMessage), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Delete_QueueMessage_Not_Found()
        {
            var messageId = Guid.NewGuid();

            _queueMessageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                                       .ReturnsAsync((QueueMessage?)null);

            Func<Task> act = async () => await _queueMessageService.DeleteAsync(messageId);

            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Queue message not found.");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Should_Dispatch_Event_Successfully()
        {
            var syncEventId = Guid.NewGuid();
            var syncEvent = new SyncEvent
            {
                SyncEventId = syncEventId,
                Payload = "{\"productId\":1}",
                EventType = "ProductCreated"
            };

            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync(syncEvent);

            await _queueMessageService.DispatchEventAsync(syncEventId);

            _publisherMock.Verify(p => p.PublishAsync(It.IsAny<string>(), "channel1"), Times.Once);
            _publisherMock.Verify(p => p.PublishAsync(It.IsAny<string>(), "channel2"), Times.Once);

            _queueMessageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<QueueMessage>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_SyncEvent_Not_Found_On_Dispatch()
        {
            var syncEventId = Guid.NewGuid();

            _syncEventRepositoryMock.Setup(r => r.GetByIdAsync(syncEventId))
                                    .ReturnsAsync((SyncEvent?)null);

            Func<Task> act = async () => await _queueMessageService.DispatchEventAsync(syncEventId);

            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Sync event not found.");

            _publisherMock.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _queueMessageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<QueueMessage>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Should_Mark_QueueMessage_As_Processed()
        {
            var messageId = Guid.NewGuid();
            var message = QueueMessageBuilder.CreateValidQueueMessage(messageId);

            _queueMessageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                                       .ReturnsAsync(message);

            await _queueMessageService.MarkAsProcessedAsync(messageId);

            message.IsProcessed.Should().BeTrue();
            _queueMessageRepositoryMock.Verify(r => r.Update(message), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_MarkAsProcessed_QueueMessage_Not_Found()
        {
            var messageId = Guid.NewGuid();

            _queueMessageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                                       .ReturnsAsync((QueueMessage?)null);

            Func<Task> act = async () => await _queueMessageService.MarkAsProcessedAsync(messageId);

            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Queue message not found.");

            _queueMessageRepositoryMock.Verify(r => r.Update(It.IsAny<QueueMessage>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
