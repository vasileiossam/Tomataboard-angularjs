using Microsoft.Extensions.Options;

namespace Tomataboard.xUnitTests.Stubs
{
    public class StubOptions<T> : IOptions<T> where T : class, new()
    {
        public StubOptions(T settings)
        {
            Value = settings;
        }

        public T Value { get; }
    }
}
