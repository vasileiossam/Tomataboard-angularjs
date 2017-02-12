using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Tomataboard.Services;

namespace Tomataboard.Logger
{
    public static class EmailLoggerExtensions
    {
        public static ILoggerFactory AddEmail(this ILoggerFactory factory,
                                              IEmailSender emailSender,
                                              IOptions<EmailLoggerOptions> options,
                                              Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new EmailLoggerProvider(filter, emailSender, options));
            return factory;
        }

        public static ILoggerFactory AddEmail(this ILoggerFactory factory, IEmailSender emailSender, IOptions<EmailLoggerOptions> options, LogLevel minLevel)
        {
            return AddEmail(
                factory,
                emailSender,
                options,
                (_, logLevel) => logLevel >= minLevel);
        }
    }
}
