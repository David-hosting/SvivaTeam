using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SvivaTeamVersion3.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions>optionsAccessor)
        {
            Options = optionsAccessor.Value;
            this.APIKey = Properties.Resources.SendGridAPIKey;
        }

        public AuthMessageSenderOptions Options { get; }
        public string APIKey { get; }
        

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(this.APIKey, subject, htmlMessage, email);
        }

        private Task Execute(string apiKey, string subject, string htmlMessage, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("svivateam1@gmail.com", "Sviva Team"),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(email));
            // Disable 
            // See Https://sendgrid.com/docs/User_Guid/Settings/tracking.html
            msg.SetClickTracking(false, false);
            return client.SendEmailAsync(msg);
        }
    }
}
