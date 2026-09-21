using UNIOOP.NotificationWorker.Messaging;

namespace UNIOOP.NotificationWorker;

public class Worker(INotificationConsumer notificationConsumer, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Notification Worker starting.");

        await notificationConsumer.StartAsync(stoppingToken);
    }
}