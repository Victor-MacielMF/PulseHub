using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PulseHub.Consumer.MercadoLivre.Messaging.Interfaces;
using PulseHub.Infrastructure.Messaging.Interfaces;
using PulseHub.Infrastructure.Messaging.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PulseHub.Consumer.MercadoLivre.Services
{
    public class MercadoLivreQueueConsumerService : BackgroundService
    {
        private readonly ILogger<MercadoLivreQueueConsumerService> _logger;
        private readonly IRabbitMQConnection _connection;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _queueName;
        private IModel _channel;

        public MercadoLivreQueueConsumerService(
            ILogger<MercadoLivreQueueConsumerService> logger,
            IRabbitMQConnection connection,
            IServiceProvider serviceProvider,
            IOptions<RabbitMQSettings> options)
        {
            _logger = logger;
            _connection = connection;
            _serviceProvider = serviceProvider;
            _queueName = options.Value.QueueName;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateChannel();

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.BasicQos(0, 1, false);

            _logger.LogInformation("Connected to RabbitMQ and listening to queue: {queue}", _queueName);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message: {message}", message);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IMercadoLivreMessageProcessor>();

                    var processed = await processor.ProcessMessageAsync(message);

                    if (processed)
                    {
                        _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                        _logger.LogInformation("Message processed successfully.");
                    }
                    else
                    {
                        _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
                        _logger.LogWarning("Message processing failed.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message.");
                    _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            base.Dispose();
        }
    }
}
