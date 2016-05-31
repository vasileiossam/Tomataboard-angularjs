using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Tomataboard.Services
{
    public class EmailSenderOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool UseSsl { get; set; }
    }
    public class SendGridOptions : EmailSenderOptions { }
    public class GmailOptions : EmailSenderOptions  { }

    /// <summary>
    /// Gmail Smtp only works with 'Less secure apps' turned on: http://www.google.com/settings/security/lesssecureapps
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private const string _fromName = "Tomataboard";
        private const string _fromAddress = "tomataboard@gmail.com";
        private ILogger<EmailSender> _logger;
        private IOptions<SendGridOptions> _sendGridOptions;
        private IOptions<GmailOptions> _gmailOptions;

        public EmailSender(ILogger<EmailSender> logger, 
            IOptions<SendGridOptions> sendGridOptions,
            IOptions<GmailOptions> gmailOptions)
        {
            _logger = logger;
            _sendGridOptions = sendGridOptions;
            _gmailOptions = gmailOptions;
        }

        public async Task<bool> TrySendEmailAsync(EmailSenderOptions options, string email, string subject, string message)
        {
            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_fromName, _fromAddress));
                mimeMessage.To.Add(new MailboxAddress(email, email));
                mimeMessage.Subject = subject;

                mimeMessage.Body = new TextPart("plain")
                {
                    Text = message
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(options.SmtpServer, options.SmtpPort, options.UseSsl);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    await client.AuthenticateAsync(options.Username, options.Password);
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// https://github.com/jstedfast/MailKit
        /// https://azure.microsoft.com/en-us/documentation/articles/sendgrid-dotnet-how-to-send-email/#what-is-the-sendgrid-email-service
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var sended = await TrySendEmailAsync(_sendGridOptions.Value, email, subject, message);
            if(!sended) await TrySendEmailAsync(_gmailOptions.Value, email, subject, message);
        }
    }
}
