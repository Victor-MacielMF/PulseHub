using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PulseHub.Infrastructure.Messaging.Interfaces;
using PulseHub.Infrastructure.Messaging.Settings;
using System;
using RabbitMQ.Client;

namespace PulseHub.Infrastructure.Messaging.Implementations
{
    public class RabbitMQConnection : IRabbitMQConnection, IDisposable
    {
        private readonly RabbitMQ.Client.IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQConnection> _logger;
        private RabbitMQ.Client.IConnection _connection;
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

        public RabbitMQ.Client.IModel CreateChannel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Não há conexão ativa com o RabbitMQ.");

            return _connection.CreateModel();
        }

        public void TryConnect()
        {
            _logger.LogInformation("Tentando conectar ao RabbitMQ...");

            _connection = _connectionFactory.CreateConnection();

            if (IsConnected)
                _logger.LogInformation("Conectado ao RabbitMQ com sucesso.");
            else
                _logger.LogError("Falha ao conectar no RabbitMQ.");
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
                _logger.LogError(ex, "Erro ao fechar a conexão RabbitMQ.");
            }
        }
    }
}
