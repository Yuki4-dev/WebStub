namespace WebStub.Services
{
    public interface ILocalApplicationDataService
    {
        public Task SetLocalDataAsync(string key, string value);

        public Task<string> GetLocalDataAsync(string key);
    }
}
