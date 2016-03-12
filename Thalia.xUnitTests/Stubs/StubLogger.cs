using System;
using Microsoft.Extensions.Logging;

namespace Thalia.xUnitTests.Stubs
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
    }
}