using RabbitMQ.Client;

namespace UniOOP.App.Messaging;

public interface IRabbitMQConnection : IAsyncDisposable
{
    Task<IConnection> GetConnectionAsync();
}