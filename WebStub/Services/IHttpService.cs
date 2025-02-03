using WebStub.Models;

namespace WebStub.Services
{
    public class HttpServiceErrorArgs(int count, string message)
    {
        public int Count { get; } = count;
        public string Message { get; } = message;
    }

    public interface IHttpService : IDisposable
    {
        bool IsListening { get; }

        event Func<HttpServiceErrorArgs, bool>? ErrorHandle;

        Task ListenAsync(string prefix, Func<HttpRequest, Task<HttpResponse>> server);

        void Stop();
    }
}
