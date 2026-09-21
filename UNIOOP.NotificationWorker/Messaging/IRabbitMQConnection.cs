using RabbitMQ.Client;

namespace UNIOOP.NotificationWorker.Messaging;

public interface IRabbitMQConnection : IAsyncDisposable
{
    Task<IConnection> GetConnectionAsync();
}