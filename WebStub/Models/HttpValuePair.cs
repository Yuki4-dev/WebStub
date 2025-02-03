namespace WebStub.Models
{
    public class HttpValuePair(string key, IEnumerable<string> values)
    {
        public string Key { get; } = key;

        public IEnumerable<string> Values { get; } = values;

        public HttpValuePair(string key, string value) : this(key, [value]) { }

        public override string ToString()
        {
            return string.Join(',', Values.Select(value => Key + ":" + value));
        }
    }
}
