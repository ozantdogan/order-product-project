using MailKit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OTD.Core.Models.Requests;
using OTD.ServiceLayer.Abstract;
using System.Text.Json;
using IMailService = OTD.ServiceLayer.Abstract.IMailService;

namespace OTD.ServiceLayer.BackgroundServices
{
    public class MailSenderBackgroundService : BackgroundService
    {
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IServiceProvider _serviceProvider;

        public MailSenderBackgroundService(IRabbitMqService rabbitMqService, IServiceProvider serviceProvider)
        {
            _rabbitMqService = rabbitMqService;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMqService.Consume("SendMail", async (message) =>
            {
                var mailRequest = JsonSerializer.Deserialize<MailRequest>(message);
                if (mailRequest != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();
                    await mailService.SendEmailAsync(mailRequest.To, mailRequest.Subject, mailRequest.Body);
                }
            });

            return Task.CompletedTask;
        }
    }
}
