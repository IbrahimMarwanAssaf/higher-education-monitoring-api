using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using UniOOP.App.Messaging.Events;

namespace UniOOP.App.Messaging;

public class GovernmentOfficerEventPublisher(IRabbitMQConnection rabbitMQConnection) : IGovernmentOfficerEventPublisher
{
    private const string ExchangeName = "unioop.events";
    private const string CreatedRoutingKey = "governmentofficer.created";

    public async Task PublishCreatedAsync(long officerId, string email,
        string role, CancellationToken cancellationToken = default)
    {
        var connection = await rabbitMQConnection.GetConnectionAsync();
        var channelOptions = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true,
            outstandingPublisherConfirmationsRateLimiter: null,
            consumerDispatchConcurrency: null);

        await using var channel = await connection.CreateChannelAsync(channelOptions, cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var eventMessage = new GovernmentOfficerCreatedEvent
        {
            OfficerId = officerId,
            Email = email,
            Role = role
        };

        var json = JsonSerializer.Serialize(eventMessage);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: CreatedRoutingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}