using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PulseHub.Infrastructure.Messaging.Implementations;
using PulseHub.Infrastructure.Messaging.Interfaces;
using PulseHub.Infrastructure.Messaging.Settings;
using RabbitMQ.Client;
using System;
using System.IO;
using Xunit;

namespace PulseHub.Infrastructure.Tests.Messaging
{
    public class RabbitMQConnectionTests : IDisposable
    {
        private readonly RabbitMQSettings _settings;
        private readonly IRabbitMQConnection _connection;

        public RabbitMQConnectionTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\PulseHub.API"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _settings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>()
                        ?? throw new InvalidOperationException("RabbitMQ settings not found in appsettings.json.");

            _settings.QueueName += "-tests";

            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                                      .CreateLogger<RabbitMQConnection>();

            _connection = new RabbitMQConnection(Options.Create(_settings), logger);
        }

        [Fact(DisplayName = "Should connect to RabbitMQ successfully")]
        public void Should_Connect_To_RabbitMQ_Successfully()
        {
            _connection.IsConnected.Should().BeTrue();
        }

        [Fact(DisplayName = "Should create a channel successfully")]
        public void Should_Create_Channel_Successfully()
        {
            using var channel = _connection.CreateChannel();
            channel.IsOpen.Should().BeTrue();
        }

        [Fact(DisplayName = "The queue should exist or be declared successfully")]
        public void Should_Declare_Or_Validate_Queue_Successfully()
        {
            using var channel = _connection.CreateChannel();

            Action act = () => channel.QueueDeclarePassive(_settings.QueueName);

            act.Should().NotThrow("the queue should exist in RabbitMQ");

            var result = channel.QueueDeclarePassive(_settings.QueueName);

            result.QueueName.Should().Be(_settings.QueueName);
            result.MessageCount.Should().BeGreaterOrEqualTo(0);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
