using Microsoft.Extensions.Logging;
using PulseHub.Consumer.MercadoLivre.Handlers;
using PulseHub.Consumer.MercadoLivre.Messaging.Interfaces;
using System;
using System.Threading.Tasks;

namespace PulseHub.Consumer.MercadoLivre.Messaging.Implementations
{
    public class MercadoLivreMessageProcessor : IMercadoLivreMessageProcessor
    {
        private readonly ILogger<MercadoLivreMessageProcessor> _logger;
        private readonly MercadoLivreQueueMessageHandler _handler;

        public MercadoLivreMessageProcessor(
            ILogger<MercadoLivreMessageProcessor> logger,
            MercadoLivreQueueMessageHandler handler)
        {
            _logger = logger;
            _handler = handler;
        }

        public async Task<bool> ProcessMessageAsync(string message)
        {
            try
            {
                _logger.LogInformation("Starting message processing...");

                var result = await _handler.HandleAsync(message);

                if (result)
                {
                    _logger.LogInformation("Message processed successfully.");
                }
                else
                {
                    _logger.LogWarning("Message failed during processing.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error while processing message.");
                return false;
            }
        }
    }
}
