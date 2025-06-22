using AutoMapper;
using Microsoft.Extensions.Configuration;
using PulseHub.Application.DTOs;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Enums;
using PulseHub.Domain.Interfaces;
using PulseHub.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PulseHub.Application.Services.Implementations
{
    public class QueueMessageService : IQueueMessageService
    {
        private readonly IQueueMessageRepository _queueMessageRepository;
        private readonly ISyncEventRepository _syncEventRepository;
        private readonly IMessagePublisher _publisher;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly List<string> _channels;

        public QueueMessageService(
            IQueueMessageRepository queueMessageRepository,
            ISyncEventRepository syncEventRepository,
            IMessagePublisher publisher,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _queueMessageRepository = queueMessageRepository;
            _syncEventRepository = syncEventRepository;
            _publisher = publisher;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

            _channels = configuration.GetSection("IntegrationChannels").Get<List<string>>()
                        ?? throw new Exception("IntegrationChannels not found in configuration.");
        }

        public async Task<IEnumerable<QueueMessageResponseDto>> GetAllAsync()
        {
            var messages = await _queueMessageRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<QueueMessageResponseDto>>(messages);
        }

        public async Task<QueueMessageResponseDto?> GetByIdAsync(Guid queueMessageId)
        {
            var message = await _queueMessageRepository.GetByIdAsync(queueMessageId);
            return message is null ? null : _mapper.Map<QueueMessageResponseDto>(message);
        }

        public async Task DeleteAsync(Guid queueMessageId)
        {
            var message = await _queueMessageRepository.GetByIdAsync(queueMessageId)
                          ?? throw new Exception("Queue message not found.");

            _queueMessageRepository.Delete(message);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DispatchEventAsync(Guid syncEventId)
        {
            var syncEvent = await _syncEventRepository.GetByIdAsync(syncEventId)
                             ?? throw new Exception("Sync event not found.");

            if (string.IsNullOrWhiteSpace(syncEvent.Payload))
                throw new Exception("Sync event does not contain payload.");

            var payload = JsonSerializer.Serialize(new IntegrationMessage<object>
            {
                EventId = syncEvent.SyncEventId,
                EventType = syncEvent.EventType,
                Timestamp = DateTime.UtcNow,
                Data = JsonSerializer.Deserialize<object>(syncEvent.Payload)
            });

            foreach (var channel in _channels)
            {
                var queueMessage = new QueueMessage
                {
                    QueueMessageId = Guid.NewGuid(),
                    SyncEventId = syncEvent.SyncEventId,
                    Payload = payload,
                    Channel = channel,
                    PublishedAt = DateTime.UtcNow,
                    Status = QueueMessageStatus.Pending,
                    RetryCount = 0
                };

                try
                {
                    await _publisher.PublishAsync(payload, channel);

                    queueMessage.Status = QueueMessageStatus.Published;
                    queueMessage.IsProcessed = false;
                    queueMessage.LastAttemptAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    queueMessage.Status = QueueMessageStatus.Failed;
                    queueMessage.ErrorMessage = ex.Message;
                    queueMessage.RetryCount = 1;
                    queueMessage.LastAttemptAt = DateTime.UtcNow;
                }

                await _queueMessageRepository.AddAsync(queueMessage);
            }

            await _unitOfWork.SaveChangesAsync();
        }



        public async Task MarkAsProcessedAsync(Guid queueMessageId)
        {
            var message = await _queueMessageRepository.GetByIdAsync(queueMessageId)
                          ?? throw new Exception("Queue message not found.");

            message.IsProcessed = true;

            _queueMessageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
