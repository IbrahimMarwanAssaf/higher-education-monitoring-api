using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using UNIOOP.NotificationWorker.Configuration;

namespace UNIOOP.NotificationWorker.Messaging;

public class SmtpEmailSender(IOptions<SmtpOptions> smtpOptions,
    IOptions<NotificationOptions> notificationOptions, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly SmtpOptions _smtpOptions = smtpOptions.Value;
    private readonly NotificationOptions _notificationOptions = notificationOptions.Value;

    public async Task SendGovernmentOfficerCreatedAsync(long officerId,
        string email, string role, CancellationToken cancellationToken = default)
    {
        if (_notificationOptions.SuperAdminEmails.Count == 0)
        {
            throw new InvalidOperationException("No Super Admin email recipients are configured.");
        }

        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_smtpOptions.FromName, _smtpOptions.FromEmail));

        foreach (var recipient in _notificationOptions.SuperAdminEmails)
        {
            message.To.Add(MailboxAddress.Parse(recipient));
        }

        message.Subject = "New Government Officer Created";

        message.Body = new TextPart("plain")
        {
            Text = $"A new Government Officer was created.{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Officer ID: {officerId}{Environment.NewLine}" +
                $"Email: {email}{Environment.NewLine}" +
                $"Role: {role}"
        };

        using var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(
            _smtpOptions.Host,
            _smtpOptions.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);

        await smtpClient.AuthenticateAsync(
            _smtpOptions.UserName,
            _smtpOptions.Password,
            cancellationToken);

        await smtpClient.SendAsync(message, cancellationToken);

        await smtpClient.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Government Officer creation email sent to {RecipientCount} Super Admin recipient(s).",
            _notificationOptions.SuperAdminEmails.Count);
    }
}
