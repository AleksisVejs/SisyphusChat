using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Communication.Email;
using System.Threading.Tasks;

namespace SisyphusChat.Core.Services
{
    internal class EmailService
    {
        private readonly EmailClient _emailClient;
        private readonly string _frontendUrl;

        public EmailService(string connectionString, string frontendUrl)
        {
            _emailClient = new EmailClient(connectionString);
            _frontendUrl = frontendUrl;
        }

        public async Task SendVerificationEmail(string toEmail, string token)
        {
            var verificationLink = $"{_frontendUrl}/Verify?token={token}";

            var subject = "SisyphusChat epasta verifikacija";
            var body = $"Verificejat savu epastu spiezot <a href='{verificationLink}'>seit</a>.";

            var emailMessage = new EmailMessage(
                senderAddress: "981c6472-13ca-4cc7-b24c-e2f87e9554d4.azurecomm.net",
                content: new EmailContent(subject)
                {
                    Html = body
                }
            );

            emailMessage.To.Add(new EmailAddress(toEmail));
            await _emailClient.SendAsync(emailMessage);
        }
    }
}
