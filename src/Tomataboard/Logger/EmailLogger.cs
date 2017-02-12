using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tomataboard.Services;
using Tomataboard.Services.Mailer;

namespace Tomataboard.Logger
{
    /// <summary>
    /// https://wildermuth.com/2016/04/22/Implementing-an-ASP-NET-Core-RC1-Logging-Provider
    /// </summary>
    public class EmailLogger : ILogger
    {
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
        private IEmailSender _emailSender;
        private readonly IOptions<EmailLoggerOptions> _options;

        public EmailLogger(string categoryName, Func<string, LogLevel, bool> filter, 
            IEmailSender emailSender,
            IOptions<EmailLoggerOptions> options)
        {
            _categoryName = categoryName;
            _filter = filter;
            _emailSender = emailSender;
            _options = options;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // Not necessary
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $@"Level: {logLevel}{message}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            _emailSender.SendEmailAsync(new EmailMessage()
            {
                To = _options.Value.AdminEmail,
                Subject ="Tomataboard Log Message",
                Text = message
            }
            );
        }
    }
}
