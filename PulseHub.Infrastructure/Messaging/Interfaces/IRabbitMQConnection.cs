using RabbitMQ.Client;

namespace PulseHub.Infrastructure.Messaging.Interfaces
{
    public interface IRabbitMQConnection
    {
        RabbitMQ.Client.IModel CreateChannel();
        bool IsConnected { get; }
        void TryConnect();
    }
}
