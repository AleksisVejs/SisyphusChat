using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Communication.Email;
using SisyphusChat.Core.Interfaces;

namespace SisyphusChat.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailClient _emailClient;

        public EmailService(EmailClient emailClient)
        {
            _emailClient = emailClient;
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string htmlMessage)
        {
            // Create the email content
            var emailContent = new EmailContent(subject)
            {
                Html = htmlMessage
            };

            // Prepare the email message
            var emailMessage = new EmailMessage("DoNotReply@981c6472-13ca-4cc7-b24c-e2f87e9554d4.azurecomm.net", recipientEmail, emailContent);

            // Send the email asynchronously
            await _emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage);
        }
    }
}
