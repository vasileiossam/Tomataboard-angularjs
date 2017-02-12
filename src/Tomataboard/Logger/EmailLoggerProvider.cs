using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Tomataboard.Services;

namespace Tomataboard.Logger
{
    public class EmailLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly IEmailSender _emailSender;
        private readonly IOptions<EmailLoggerOptions> _options;

        public EmailLoggerProvider(Func<string, LogLevel, bool> filter, IEmailSender emailSender, IOptions<EmailLoggerOptions> options)
        {
            _emailSender = emailSender;
            _filter = filter;
            _options = options;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new EmailLogger(categoryName, _filter, _emailSender, _options);
        }

        public void Dispose()
        {
        }
    }
}
