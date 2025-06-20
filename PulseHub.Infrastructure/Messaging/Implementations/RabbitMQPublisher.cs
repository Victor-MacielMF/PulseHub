using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PulseHub.Infrastructure.Messaging.Interfaces;
using PulseHub.Infrastructure.Messaging.Settings;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PulseHub.Infrastructure.Messaging.Implementations
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly IRabbitMQConnection _connection;
        private readonly RabbitMQSettings _settings;
        private readonly ILogger<RabbitMQPublisher> _logger;

        public RabbitMQPublisher(
            IRabbitMQConnection connection,
            IOptions<RabbitMQSettings> settings,
            ILogger<RabbitMQPublisher> logger)
        {
            _connection = connection;
            _settings = settings.Value;
            _logger = logger;
        }

        public Task PublishAsync(string message)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using var channel = _connection.CreateChannel();

            try
            {
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: _settings.QueueName,
                    basicProperties: null,
                    body: body
                );

                _logger.LogInformation("Message published to queue {QueueName}: {Message}", _settings.QueueName, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to queue {QueueName}", _settings.QueueName);
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
