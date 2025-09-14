using WebStub.Core;

namespace WebStub.Services
{
    public class LocalApplicationDataService : ILocalApplicationDataService
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        public LocalApplicationDataService()
        {
            _folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WebStub");
            _filePath = Path.Combine(_folderPath, "web-stub-local-data.json");
        }

        public async Task<string> GetLocalDataAsync(string key)
        {
            if (!File.Exists(_filePath))
            {
                return "";
            }

            var valueJson = await LoadTextAsync();
            if (!string.IsNullOrEmpty(valueJson))
            {
                var valueMap = Json.FromJson<Dictionary<string, string>>(valueJson);
                if (valueMap != null && valueMap.TryGetValue(key, out string? value))
                {
                    return value;
                }
            }

            return "";
        }

        public async Task SetLocalDataAsync(string key, string value)
        {
            CreateFile();

            var valueJson = await LoadTextAsync();

            Dictionary<string, string>? valueMap = null;
            if (!string.IsNullOrEmpty(valueJson))
            {
                valueMap = Json.FromJson<Dictionary<string, string>>(valueJson);
            }

            if (valueMap == null)
            {
                valueMap = [];
            }

            valueMap[key] = value;
            await SaveTextAsync(Json.ToJson(valueMap));
        }

        public Task DeleteLocalDataAsync()
        {
            if (!File.Exists(_filePath))
            {
                return Task.CompletedTask;
            }

            File.Delete(_filePath);
            Directory.Delete(_folderPath);
            return Task.CompletedTask;
        }


        private async Task SaveTextAsync(string text)
        {
            try
            {
                await File.WriteAllTextAsync(_filePath, text);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        private async Task<string?> LoadTextAsync()
        {
            try
            {
                return await File.ReadAllTextAsync(_filePath);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return null;
            }
        }

        private void CreateFile()
        {
            if (File.Exists(_filePath))
            {
                return;
            }

            if (!Directory.Exists(_folderPath))
            {
                _ = Directory.CreateDirectory(_folderPath);
            }

            File.Create(_filePath).Close();
        }
    }
}
