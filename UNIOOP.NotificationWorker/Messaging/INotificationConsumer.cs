namespace UNIOOP.NotificationWorker.Messaging;

public interface INotificationConsumer
{
    Task StartAsync(CancellationToken cancellationToken);
}
