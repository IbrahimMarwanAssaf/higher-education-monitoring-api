using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UNIOOP.NotificationWorker.Messaging.Events;

namespace UNIOOP.NotificationWorker.Messaging;

public class NotificationConsumer(IRabbitMQConnection rabbitMQConnection, IEmailSender emailSender,
    ILogger<NotificationConsumer> logger) : INotificationConsumer
{
    private readonly IEmailSender _emailSender = emailSender;

    private const string ExchangeName = "unioop.events";
    private const string QueueName = "unioop.notification";
    private const string BindingKey = "governmentofficer.created";

    private const string DeadLetterExchangeName = "unioop.dead";
    private const string DeadLetterQueueName = "unioop.notification.dead";
    private const string DeadLetterRoutingKey = "governmentofficer.created.dead";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var connection = await rabbitMQConnection.GetConnectionAsync();

        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic, durable: true,
            autoDelete: false, cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(exchange: DeadLetterExchangeName, type: ExchangeType.Direct, durable: true,
            autoDelete: false, cancellationToken: cancellationToken);

        var queueArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = DeadLetterExchangeName,
            ["x-dead-letter-routing-key"] = DeadLetterRoutingKey
        };

        await channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false,
            autoDelete: false, arguments: queueArguments, cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: DeadLetterQueueName, durable: true, exclusive: false,
            autoDelete: false, cancellationToken: cancellationToken);

        await channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName,
            routingKey: BindingKey, cancellationToken: cancellationToken);

        await channel.QueueBindAsync(queue: DeadLetterQueueName, exchange: DeadLetterExchangeName,
            routingKey: DeadLetterRoutingKey, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var officerEvent = JsonSerializer.Deserialize<GovernmentOfficerCreatedEvent>(json);

                if (officerEvent is null)
                {
                    logger.LogWarning("Received an invalid GovernmentOfficerCreated event.");

                    await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);

                    return;
                }

                await _emailSender.SendGovernmentOfficerCreatedAsync(officerEvent.OfficerId, officerEvent.Email,
                    officerEvent.Role, cancellationToken);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing GovernmentOfficerCreated event.");

                await channel.BasicNackAsync(eventArgs.DeliveryTag,
                    multiple: false, requeue: false);
            }
        };

        await channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);

        logger.LogInformation("Notification consumer started. Waiting for messages.");

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}




