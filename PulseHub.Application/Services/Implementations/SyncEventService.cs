using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Enums;
using PulseHub.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PulseHub.Application.Services.Implementations
{
    public class SyncEventService : ISyncEventService
    {
        private readonly ISyncEventRepository _syncEventRepository;
        private readonly IQueueMessageService _queueMessageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SyncEventService(
            ISyncEventRepository syncEventRepository,
            IQueueMessageService queueMessageService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _syncEventRepository = syncEventRepository;
            _queueMessageService = queueMessageService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SyncEventResponseDto>> GetAllAsync()
        {
            var events = await _syncEventRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SyncEventResponseDto>>(events);
        }

        public async Task<SyncEventResponseDto?> GetByIdAsync(Guid syncEventId)
        {
            var syncEvent = await _syncEventRepository.GetByIdAsync(syncEventId);
            return syncEvent is null ? null : _mapper.Map<SyncEventResponseDto>(syncEvent);
        }

        public async Task DeleteAsync(Guid syncEventId)
        {
            var syncEvent = await _syncEventRepository.GetByIdAsync(syncEventId)
                             ?? throw new Exception("Sync event not found.");

            _syncEventRepository.Delete(syncEvent);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<SyncEvent> RegisterSyncEventAsync(string eventType, object data)
        {
            var payload = JsonSerializer.Serialize(data);

            var syncEvent = new SyncEvent
            {
                SyncEventId = Guid.NewGuid(),
                ProductId = ExtractProductId(data),
                EventType = eventType,
                EventDate = DateTime.UtcNow,
                Status = SyncEventStatus.Pending,
                Message = GenerateEventMessage(eventType, data),
                Payload = payload
            };

            await _syncEventRepository.AddAsync(syncEvent);
            await _unitOfWork.SaveChangesAsync();

            return syncEvent;
        }

        public async Task PublishToIntegrationAsync(Guid syncEventId)
        {
            await _queueMessageService.DispatchEventAsync(syncEventId);
        }

        public async Task MarkAsProcessedAsync(Guid syncEventId)
        {
            var syncEvent = await _syncEventRepository.GetByIdAsync(syncEventId)
                             ?? throw new Exception("Sync event not found.");

            syncEvent.Status = SyncEventStatus.Completed;

            _syncEventRepository.Update(syncEvent);
            await _unitOfWork.SaveChangesAsync();
        }

        private string GenerateEventMessage(string eventType, object data)
        {
            var productId = ExtractProductId(data);

            return eventType switch
            {
                "ProductCreated" => $"Event: ProductCreated | Product ID: {productId} | Sent to integration queue.",
                "ProductUpdated" => $"Event: ProductUpdated | Product ID: {productId} | Sent to integration queue.",
                "ProductDeleted" => $"Event: ProductDeleted | Product ID: {productId} | Sent to integration queue.",
                _ => $"Event: {eventType} | Product ID: {productId} | Sent to integration queue."
            };
        }

        private Guid ExtractProductId(object data)
        {
            var prop = data.GetType().GetProperty("ProductId");
            if (prop != null)
            {
                var value = prop.GetValue(data);
                if (value is Guid guid)
                    return guid;
            }

            return Guid.Empty;
        }
    }
}
