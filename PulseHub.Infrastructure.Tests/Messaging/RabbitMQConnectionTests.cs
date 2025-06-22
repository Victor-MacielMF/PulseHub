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
    public class RabbitMQConnectionTests
    {
        private readonly RabbitMQSettings _settings;

        public RabbitMQConnectionTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\PulseHub.API"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _settings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>()
                       ?? throw new InvalidOperationException("RabbitMQ settings not found in appsettings.json.");
            _settings.QueueName += "-tests";
        }

        private IRabbitMQConnection CreateConnection()
        {
            var options = Options.Create(_settings);
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                                      .CreateLogger<RabbitMQConnection>();

            return new RabbitMQConnection(options, logger);
        }

        [Fact(DisplayName = "Should connect to RabbitMQ successfully")]
        public void Should_Connect_To_RabbitMQ_Successfully()
        {
            var connection = CreateConnection();
            connection.IsConnected.Should().BeTrue();
        }

        [Fact(DisplayName = "Should create a channel successfully")]
        public void Should_Create_Channel_Successfully()
        {
            var connection = CreateConnection();
            using var channel = connection.CreateChannel();
            channel.IsOpen.Should().BeTrue();
        }

        [Fact(DisplayName = "The queue sync-events-queue should exist")]
        public void Queue_Should_Exist()
        {
            var connection = CreateConnection();
            using var channel = connection.CreateChannel();

            var result = channel.QueueDeclarePassive(_settings.QueueName);

            result.QueueName.Should().Be(_settings.QueueName);
            result.MessageCount.Should().BeGreaterOrEqualTo(0);
        }
    }
}
