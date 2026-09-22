namespace UNIOOP.NotificationWorker.Messaging;

public interface IEmailSender
{
    Task SendGovernmentOfficerCreatedAsync(long officerId, string email, string role,
        CancellationToken cancellationToken = default);
}
