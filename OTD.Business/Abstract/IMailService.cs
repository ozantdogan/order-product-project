﻿namespace OTD.ServiceLayer.Abstract
{
    public interface IMailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}