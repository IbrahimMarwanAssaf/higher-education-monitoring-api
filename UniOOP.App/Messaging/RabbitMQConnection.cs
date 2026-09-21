using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UniOOP.App.Configuration;

namespace UniOOP.App.Messaging;

public class RabbitMQConnection(IOptions<RabbitMQOptions> options) : IRabbitMQConnection
{
    private readonly RabbitMQOptions _options = options.Value;
    private IConnection? _connection;

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is not null && _connection.IsOpen)
        {
            return _connection;
        }

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        _connection = await factory.CreateConnectionAsync();

        return _connection;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}