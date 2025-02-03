namespace WebStub.Models
{
    public class HttpRequest(string method, string uri, string body, IEnumerable<HttpValuePair> headers, IEnumerable<HttpValuePair> cookies, IEnumerable<HttpValuePair> parameters)
    {
        public string Method { get; } = method;

        public string Uri { get; } = uri;

        public string Body { get; } = body;

        public IEnumerable<HttpValuePair> Headers { get; } = headers;

        public IEnumerable<HttpValuePair> Cookies { get; } = cookies;

        public IEnumerable<HttpValuePair> Parameters { get; } = parameters;

        public HttpRequestJson ToJson()
        {
            return new HttpRequestJson()
            {
                method = Method,
                body = Body,
                uri = Uri,
                header = Headers.ToDictionary(h => h.Key, h => h.Values.ToArray()),
                cookie = Cookies.ToDictionary(c => c.Key, c => c.Values.ToArray()),
                parameter = Parameters.ToDictionary(p => p.Key, p => p.Values.ToArray()),
            };

        }
    }
}
