using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PulseHub.Domain.Interfaces;
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
    public class RabbitMQPublisherTests
    {
        private readonly RabbitMQSettings _settings;

        public RabbitMQPublisherTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\PulseHub.API"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _settings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>()
                       ?? throw new InvalidOperationException("RabbitMQ settings not found in appsettings.json.");
        }

        private IMessagePublisher CreatePublisher()
        {
            var connection = new RabbitMQConnection(
                Options.Create(_settings),
                LoggerFactory.Create(builder => builder.AddConsole())
                             .CreateLogger<RabbitMQConnection>());

            var publisher = new RabbitMQPublisher(
                connection,
                Options.Create(_settings),
                LoggerFactory.Create(builder => builder.AddConsole())
                             .CreateLogger<RabbitMQPublisher>());

            return publisher;
        }

        [Fact(DisplayName = "Should publish a message successfully")]
        public async Task Should_Publish_Message_Successfully()
        {
            var publisher = CreatePublisher();

            var message = new
            {
                EventId = Guid.NewGuid(),
                EventType = "TestMessage",
                Timestamp = DateTime.UtcNow,
                Data = new { ProductId = 1, Name = "Test" }
            };

            var json = JsonSerializer.Serialize(message);

            Func<Task> act = async () => await publisher.PublishAsync(json, "tests");

            await act.Should().NotThrowAsync();
        }
    }
}
