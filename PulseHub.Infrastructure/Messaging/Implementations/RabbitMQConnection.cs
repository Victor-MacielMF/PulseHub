using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PulseHub.Infrastructure.Messaging.Interfaces;
using PulseHub.Infrastructure.Messaging.Settings;
using RabbitMQ.Client;
using System;

namespace PulseHub.Infrastructure.Messaging.Implementations
{
    public class RabbitMQConnection : IRabbitMQConnection, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQConnection> _logger;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection(IOptions<RabbitMQSettings> options, ILogger<RabbitMQConnection> logger)
        {
            _logger = logger;

            _connectionFactory = new ConnectionFactory
            {
                HostName = options.Value.Host,
                Port = options.Value.Port,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            TryConnect();
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public IModel CreateChannel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("No active RabbitMQ connection.");

            return _connection.CreateModel();
        }

        public void TryConnect()
        {
            _logger.LogInformation("Attempting to connect to RabbitMQ...");

            _connection = _connectionFactory.CreateConnection();

            if (IsConnected)
                _logger.LogInformation("Successfully connected to RabbitMQ.");
            else
                _logger.LogError("Failed to connect to RabbitMQ.");
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                _connection.Dispose();
                _disposed = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while closing RabbitMQ connection.");
            }
        }
    }
}
