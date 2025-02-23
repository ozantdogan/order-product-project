using Microsoft.Extensions.Configuration;
using MimeKit;
using OTD.ServiceLayer.Abstract;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

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
            var fromName = _configuration["MailSettings:FromName"];
            var fromEmail = _configuration["MailSettings:FromEmail"];
            var smtpServer = _configuration["MailSettings:SmtpServer"];
            var smtpPort = _configuration["MailSettings:SmtpPort"];
            var smtpUser = _configuration["MailSettings:SmtpUser"];
            var smtpPassword = _configuration["MailSettings:SmtpPassword"];

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(fromName, fromEmail));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, 587, false);
            await smtp.AuthenticateAsync(smtpUser, smtpPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
