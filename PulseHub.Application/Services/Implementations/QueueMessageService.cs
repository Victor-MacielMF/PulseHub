using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseHub.Application.Services.Implementations
{
    public class QueueMessageService : IQueueMessageService
    {
        private readonly IQueueMessageRepository _queueMessageRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public QueueMessageService(
            IQueueMessageRepository queueMessageRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _queueMessageRepository = queueMessageRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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
            var message = await _queueMessageRepository.GetByIdAsync(queueMessageId);

            if (message is null)
                throw new Exception("Queue message not found.");

            _queueMessageRepository.Delete(message);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Registrar uma mensagem publicada na fila para um canal específico.
        /// </summary>
        public async Task RegisterQueueMessageAsync(Guid syncEventId, string payload, string channel)
        {
            var message = new QueueMessage
            {
                QueueMessageId = Guid.NewGuid(),
                SyncEventId = syncEventId,
                Payload = payload,
                Channel = channel,
                PublishedAt = DateTime.UtcNow,
                IsProcessed = false
            };

            await _queueMessageRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Atualizar o status de uma mensagem como processada.
        /// </summary>
        public async Task MarkAsProcessedAsync(Guid queueMessageId)
        {
            var message = await _queueMessageRepository.GetByIdAsync(queueMessageId);

            if (message is null)
                throw new Exception("Queue message not found.");

            message.IsProcessed = true;

            _queueMessageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
