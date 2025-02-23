using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Serilog;
using OTD.ServiceLayer.Abstract;

namespace OTD.ServiceLayer.Concrete
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var fromName = _configuration["MailSettings:FromName"];
                var fromEmail = _configuration["MailSettings:FromEmail"];
                var smtpServer = _configuration["MailSettings:SmtpServer"];
                var smtpPortString = _configuration["MailSettings:SmtpPort"];

                if (!int.TryParse(smtpPortString, out int smtpPort))
                {
                    Log.Error("Invalid SMTP Port: {SmtpPort}", smtpPortString);
                    throw new Exception($"Invalid SMTP port: {smtpPortString}");
                }

                var smtpUser = _configuration["MailSettings:SmtpUser"];
                var smtpPassword = _configuration["MailSettings:SmtpPassword"];

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(fromName, fromEmail));
                email.To.Add(new MailboxAddress("", to));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Timeout = 10000;

                Log.Information("Connecting to SMTP server {SmtpServer}:{SmtpPort}", smtpServer, smtpPort);

                await smtp.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(smtpUser, smtpPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                Log.Information("Email sent successfully to {Recipient}", to);
            }
            catch (SmtpCommandException smtpEx)
            {
                Log.Error(smtpEx, "SMTP Command Error: {StatusCode} - {Message}", smtpEx.StatusCode, smtpEx.Message);
            }
            catch (SmtpProtocolException smtpProtoEx)
            {
                Log.Error(smtpProtoEx, "SMTP Protocol Error: {Message}", smtpProtoEx.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send email to {Recipient} with subject {Subject}", to, subject);
            }
        }
    }
}
