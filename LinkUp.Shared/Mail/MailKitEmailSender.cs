using LinkUp.Shared.Mail;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LinkUp.Infrastructure.Shared.Mail
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly MailSettings _cfg;
        public MailKitEmailSender(IOptions<MailSettings> options) => _cfg = options.Value;

        public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_cfg.DisplayName, _cfg.EmailFrom));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_cfg.SmtpHost, _cfg.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_cfg.SmtpUser, _cfg.SmtpPass, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
