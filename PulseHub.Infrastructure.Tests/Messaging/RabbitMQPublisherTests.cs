using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PulseHub.Infrastructure.Messaging.Implementations;
using PulseHub.Infrastructure.Messaging.Interfaces;
using PulseHub.Infrastructure.Messaging.Settings;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace PulseHub.Infrastructure.Tests.Messaging
{
    public class RabbitMQPublisherTests : IDisposable
    {
        private readonly RabbitMQSettings _settings;
        private readonly IRabbitMQConnection _connection;
        private readonly RabbitMQPublisher _publisher;

        public RabbitMQPublisherTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\PulseHub.API"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _settings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>()
                        ?? throw new InvalidOperationException("RabbitMQ settings not found.");

            _settings.QueueName += "-tests";

            var loggerConnection = LoggerFactory.Create(builder => builder.AddConsole())
                                                 .CreateLogger<RabbitMQConnection>();

            _connection = new RabbitMQConnection(Options.Create(_settings), loggerConnection);

            var loggerPublisher = LoggerFactory.Create(builder => builder.AddConsole())
                                                .CreateLogger<RabbitMQPublisher>();

            _publisher = new RabbitMQPublisher(_connection, Options.Create(_settings), loggerPublisher);
        }

        [Fact(DisplayName = "Should publish a message successfully")]
        public async Task Should_Publish_Message_Successfully()
        {
            // Arrange
            var message = new
            {
                EventId = Guid.NewGuid(),
                EventType = "TestMessage",
                Timestamp = DateTime.UtcNow,
                Data = new { ProductId = 1, Name = "TestProduct" }
            };

            var json = JsonSerializer.Serialize(message);
            var testChannel = "tests";

            // Act
            Func<Task> act = async () => await _publisher.PublishAsync(json, testChannel);

            // Assert
            await act.Should().NotThrowAsync();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
