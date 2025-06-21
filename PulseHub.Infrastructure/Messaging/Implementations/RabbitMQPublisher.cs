using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PulseHub.Domain.Interfaces;
using PulseHub.Domain.Messaging;
using PulseHub.Infrastructure.Messaging.Interfaces;
using PulseHub.Infrastructure.Messaging.Settings;
using RabbitMQ.Client;
using System;
using System.Text;
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

        public Task PublishAsync(string message, string channel)
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            using var channelModel = _connection.CreateChannel();

            var queueName = $"{_settings.QueueName}-{channel.ToLower()}";

            channelModel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            try
            {
                var body = Encoding.UTF8.GetBytes(message);

                channelModel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: null,
                    body: body
                );

                _logger.LogInformation("Message published to queue {QueueName}: {Message}", queueName, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to queue {QueueName}", queueName);
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
