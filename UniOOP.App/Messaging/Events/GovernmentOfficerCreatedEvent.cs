namespace UniOOP.App.Messaging.Events;

public class GovernmentOfficerCreatedEvent
{
    public long OfficerId { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}