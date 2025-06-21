using AutoMapper;
using PulseHub.Application.DTOs;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Interfaces;
using PulseHub.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PulseHub.Application.Services.Implementations
{
    public class SyncEventService : ISyncEventService
    {
        private readonly ISyncEventRepository _syncEventRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessagePublisher _publisher;
        private readonly IMapper _mapper;
        private readonly IQueueMessageService _queueMessageService;

        public SyncEventService(
            ISyncEventRepository syncEventRepository,
            IUnitOfWork unitOfWork,
            IMessagePublisher publisher,
            IMapper mapper,
            IQueueMessageService queueMessageService)
        {
            _syncEventRepository = syncEventRepository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
            _mapper = mapper;
            _queueMessageService = queueMessageService;
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
            var syncEvent = await _syncEventRepository.GetByIdAsync(syncEventId);

            if (syncEvent is null)
                throw new Exception("Evento de sincronização não encontrado.");

            _syncEventRepository.Delete(syncEvent);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RegisterSyncEventAsync(string eventType, object data)
        {
            var json = JsonSerializer.Serialize(data);

            var syncEvent = new SyncEvent
            {
                SyncEventId = Guid.NewGuid(),
                ProductId = ExtractProductId(data),
                EventType = eventType,
                EventDate = DateTime.UtcNow,
                Status = "Pending",
                Message = GenerateEventMessage(eventType, data)
            };

            await _syncEventRepository.AddAsync(syncEvent);
            await _unitOfWork.SaveChangesAsync();

            // Lista de canais que você quer processar
            //var channels = new List<string> { "MercadoLivre", "Shopee", "Magalu" };
            var channels = new List<string> {"MercadoLivre"};

            foreach (var channel in channels)
            {
                // 🔸 Publica na fila (exemplo: fila específica do canal)
                await _publisher.PublishAsync(json, channel);

                // 🔸 Salva o histórico da mensagem na fila para esse canal
                await _queueMessageService.RegisterQueueMessageAsync(syncEvent.SyncEventId, json, channel);
            }
        }



        private string GenerateEventMessage(string eventType, object data)
        {
            string productName = string.Empty;
            Guid productId = Guid.Empty;

            if (data is ProductResponseDto productDto)
            {
                productName = productDto.Name;
                productId = productDto.ProductId;
            }
            else
            {
                var nameProp = data.GetType().GetProperty("Name");
                if (nameProp != null)
                    productName = nameProp.GetValue(data)?.ToString() ?? string.Empty;

                var idProp = data.GetType().GetProperty("ProductId");
                if (idProp != null)
                {
                    var idValue = idProp.GetValue(data);
                    if (idValue is Guid guid)
                        productId = guid;
                }
            }

            return eventType switch
            {
                "ProductCreated" => $"Event: ProductCreated — Product ID: {productId} — A creation event was published to the integration queue.",
                "ProductUpdated" => $"Event: ProductUpdated — Product ID: {productId} — An update event was published to the integration queue.",
                "ProductDeleted" => $"Event: ProductDeleted — Product ID: {productId} — A deletion event was published to the integration queue.",
                _ => $"Event: {eventType} — Product ID: {productId} — Event published to the integration queue."
            };
        }


        private Guid ExtractProductId(object data)
        {
            if (data is ProductResponseDto productDto)
                return productDto.ProductId;

            // Tentativa de acessar via reflexão (para objetos anônimos, como Delete)
            var property = data.GetType().GetProperty("ProductId");
            if (property != null)
            {
                var value = property.GetValue(data);
                if (value is Guid guid)
                    return guid;
            }

            return Guid.Empty;
        }

    }
}
