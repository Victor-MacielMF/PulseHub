using RabbitMQ.Client;
using System;

namespace PulseHub.Infrastructure.Messaging.Interfaces
{
    public interface IRabbitMQConnection : IDisposable
    {
        RabbitMQ.Client.IModel CreateChannel();
        bool IsConnected { get; }
        void TryConnect();
    }
}
