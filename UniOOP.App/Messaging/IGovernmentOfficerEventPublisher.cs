namespace UniOOP.App.Messaging;

public interface IGovernmentOfficerEventPublisher
{
    Task PublishCreatedAsync(long officerId, string email, string role, CancellationToken cancellationToken = default);
}