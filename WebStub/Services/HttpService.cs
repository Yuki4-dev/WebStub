using System.Net;
using System.Text;
using WebStub.Core;
using WebStub.Models;

namespace WebStub.Services
{
    public class HttpService(ILogger logger) : IHttpService
    {
        private readonly ILogger logger = logger;

        private HttpListener? _HttpListener = null;

        public event Func<HttpServiceErrorArgs, bool>? ErrorHandle;

        public bool IsListening => _HttpListener?.IsListening == true;

        public bool IsDisposed { get; set; } = false;

        public async Task ListenAsync(string prefix, Func<HttpRequest, Task<HttpResponse>> server)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            if (IsListening)
            {
                throw new InvalidOperationException("Aleady Start.");
            }

            logger.Info($"Open server -> {prefix}");
            _HttpListener = new HttpListener();
            _HttpListener.Prefixes.Add(prefix);
            _HttpListener.Start();

            logger.Info($"Listen server -> {prefix}");
            await Task.Run(async () =>
            {
                var count = 1;
                while (IsListening)
                {
                    try
                    {
                        HttpListenerContext context = await _HttpListener.GetContextAsync();
                        await Server(context, server);
                    }
                    catch (Exception ex)
                    {
                        Error(ex);
                        if (ErrorHandle == null || ErrorHandle.Invoke(new(count++, ex.Message)))
                        {
                            throw;
                        }
                    }
                }
            });
        }

        private async Task Server(HttpListenerContext context, Func<HttpRequest, Task<HttpResponse>> server)
        {
            HttpListenerRequest request = context.Request;
            logger.Info($"Recieve Url -> {request.Url}");
            logger.Info($"Remote EndPoint -> {request.RemoteEndPoint}");
            logger.Info($"Local EndPoint -> {request.LocalEndPoint}");
            logger.Info($"Content Length -> {request.ContentLength64}");

            var requestBody = string.Empty;
            if (request.HasEntityBody)
            {
                using var stream = request.InputStream;
                using var reader = new StreamReader(stream, request.ContentEncoding);
                requestBody = reader.ReadToEnd();
            }

            var headers = new List<HttpValuePair>();
            foreach (string? key in request.Headers.AllKeys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    var values = (request.Headers[key] ?? string.Empty).Split(',');
                    headers.Add(new HttpValuePair(key, values));
                }
            }

            var cookies = new List<HttpValuePair>();
            if (request.Cookies != null)
            {
                foreach (var cookie in request.Cookies.Cast<Cookie>())
                {
                    cookies.Add(new HttpValuePair(cookie.Name, cookie.Value));
                }
            }

            var parameters = new List<HttpValuePair>();
            if (request.QueryString.HasKeys())
            {
                foreach (var key in request.QueryString.AllKeys)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        var values = request.QueryString.GetValues(key) ?? [];
                        parameters.Add(new HttpValuePair(key, values));
                    }
                }
            }

            var httpRequest = new HttpRequest(request.HttpMethod, request.Url?.AbsolutePath ?? string.Empty, requestBody, headers, cookies, parameters);
            logger.Info($"Request -> {Json.ToJson(httpRequest)}");

            var httpResponse = await server.Invoke(httpRequest);
            logger.Info($"Response -> {Json.ToJson(httpResponse)}");

            using (var response = context.Response)
            {
                response.StatusCode = httpResponse.Status;

                foreach (var headerPair in httpResponse.Headers)
                {
                    foreach (string value in headerPair.Values)
                    {
                        response.Headers.Add(headerPair.Key, value);
                    }
                }

                foreach (var cookiePair in httpResponse.Cookies)
                {
                    foreach (string value in cookiePair.Values)
                    {
                        response.Cookies.Add(new Cookie(cookiePair.Key, value));
                    }
                }

                using var stream = response.OutputStream;
                using var writer = new StreamWriter(stream, response?.ContentEncoding ?? Encoding.UTF8);
                writer.Write(httpResponse.Body);
            }
        }

        public void Stop()
        {
            if (IsDisposed || _HttpListener == null)
            {
                return;
            }

            try
            {
                _HttpListener.Stop();
                _HttpListener.Close();
            }
            catch (Exception ex)
            {
                Error(ex);
            }
            finally
            {
                _HttpListener = null;
            }
        }

        public void Dispose()
        {
            Stop();
            IsDisposed = true;
        }

        private void Error(Exception ex)
        {
            logger.Error(ex.Message);
#if DEBUG
            logger.Error(ex.StackTrace ?? string.Empty);
#endif
        }
    }
}