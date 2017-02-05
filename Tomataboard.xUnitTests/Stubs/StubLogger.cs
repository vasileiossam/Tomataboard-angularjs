using Microsoft.Extensions.Logging;
using System;

namespace Tomataboard.xUnitTests.Stubs
{
    public class StubLogger<T> : ILogger<T>
    {
        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            // no op
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return null;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}