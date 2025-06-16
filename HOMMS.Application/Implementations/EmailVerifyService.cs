using HOMMS.Application.BaseServices;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace HOMMS.Application.Implementations
{
    public class EmailVerifyService : IEmailVerifyService
    {
        private readonly IConfiguration _configuration;

        public EmailVerifyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task SendEmailAsync(MessageDto message)
        {
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
               _configuration["EmailSettings:FromEmail"],
               _configuration["EmailSettings:Password"]
               )
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"], _configuration["EmailSettings:FromName"]),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(message.To);
            try
            {
                await smtp.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                // log lỗi hoặc throw exception rõ ràng
                throw new InvalidOperationException("Failed to send email.", ex);
            }




        }
    }
}
