using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PulseHub.Consumer.MercadoLivre.Messaging.Models;
using PulseHub.Domain.Entities;
using PulseHub.Domain.Enums;
using PulseHub.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PulseHub.Consumer.MercadoLivre.Handlers
{
    public class MercadoLivreQueueMessageHandler
    {
        private readonly ILogger<MercadoLivreQueueMessageHandler> _logger;
        private readonly IQueueMessageRepository _queueMessageRepository;
        private readonly ISyncEventRepository _syncEventRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MercadoLivreQueueMessageHandler(
            ILogger<MercadoLivreQueueMessageHandler> logger,
            IQueueMessageRepository queueMessageRepository,
            ISyncEventRepository syncEventRepository,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _queueMessageRepository = queueMessageRepository;
            _syncEventRepository = syncEventRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HandleAsync(string message)
        {
            try
            {
                var payload = JsonConvert.DeserializeObject<QueueMessagePayload>(message);

                if (payload is null)
                {
                    _logger.LogError("Invalid payload. Message: {message}", message);
                    return false;
                }

                _logger.LogInformation("Handling EventId: {id}", payload.EventId);
                await Task.Delay(1000);
                var queueMessage = await _queueMessageRepository.GetByIdAsync(payload.QueueMessageId);
                if (queueMessage == null)
                {
                    _logger.LogError("QueueMessage not found. Id: {id}", payload.QueueMessageId);
                    return false;
                }

                // Simula sucesso ou falha
                var random = new Random();
                var isSuccess = random.Next(0, 2) == 1;

                if (isSuccess)
                {
                    //bem aqui tem que colocar a lógica de atualizar no banco de dados.? já tem o save async, por que n atualizou?
                    queueMessage.SetAsProcessed();
                    _queueMessageRepository.Update(queueMessage);
                    _logger.LogInformation("QueueMessage {id} processed successfully", queueMessage.QueueMessageId);
                }
                else
                {
                    queueMessage.SetAsFailed("Simulated error while processing Mercado Livre");
                    _queueMessageRepository.Update(queueMessage);
                    _logger.LogWarning("QueueMessage {id} failed during processing", queueMessage.QueueMessageId);
                }

                await _unitOfWork.SaveChangesAsync();

                // Verificar se todos os QueueMessages do SyncEvent foram processados
                var allMessages = await _queueMessageRepository.GetBySyncEventIdAsync(queueMessage.SyncEventId);

                var allProcessed = allMessages.All(x => x.Status == QueueMessageStatus.Processed);

                if (allProcessed)
                {
                    var syncEvent = await _syncEventRepository.GetByIdAsync(queueMessage.SyncEventId);

                    if (syncEvent != null)
                    {
                        syncEvent.SetAsCompleted();

                        _syncEventRepository.Update(syncEvent);
                        await _unitOfWork.SaveChangesAsync();
                        _logger.LogInformation("SyncEvent {id} marked as Completed", syncEvent.SyncEventId);
                    }
                }

                return isSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling message.");
                return false;
            }
        }
    }
}
