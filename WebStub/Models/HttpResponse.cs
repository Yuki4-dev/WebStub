namespace WebStub.Models
{
    public class HttpResponse(int status, string body, IEnumerable<HttpValuePair> headers, IEnumerable<HttpValuePair> cookies)
    {
        public static HttpResponse Empty { get; } = new HttpResponse(200, "");

        public int Status { get; } = status;

        public string Body { get; } = body;

        public IEnumerable<HttpValuePair> Headers { get; } = headers;

        public IEnumerable<HttpValuePair> Cookies { get; } = cookies;

        public HttpResponse(int status, string body) : this(status, body, new List<HttpValuePair>(), new List<HttpValuePair>()) { }

        public static HttpResponse FromJson(HttpResponseJson json)
        {
            return new HttpResponse(json.status ?? 200,
                json.body ?? string.Empty,
                json.header?.Select(h => new HttpValuePair(h.Key, h.Value)) ?? Array.Empty<HttpValuePair>(),
                json.cookie?.Select(c => new HttpValuePair(c.Key, c.Value)) ?? Array.Empty<HttpValuePair>());
        }
    }
}
