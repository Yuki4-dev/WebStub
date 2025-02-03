using WebStub.Core;

namespace WebStub.Services
{
    public class LocalApplicationDataService : ILocalApplicationDataService
    {
        private const string local_application_folder_name = "WebStub";
        private const string local_application_file_name = "web-stub-local-data.json";

        private string? _filePath;

        public async Task<string> GetLocalDataAsync(string key)
        {
            string localFilePath = GetOrCreateFilePath();
            string valueJson = await File.ReadAllTextAsync(localFilePath);
            if (!string.IsNullOrEmpty(valueJson))
            {
                Dictionary<string, string>? valueMap = Json.FromJson<Dictionary<string, string>>(valueJson);
                if (valueMap != null)
                {
                    if (valueMap.ContainsKey(key))
                    {
                        return valueMap[key];
                    }
                }
            }

            return "";
        }

        public async Task SetLocalDataAsync(string key, string value)
        {
            string localFilePath = GetOrCreateFilePath();

            Dictionary<string, string>? valueMap;
            string valueJson = await File.ReadAllTextAsync(localFilePath);
            if (string.IsNullOrEmpty(valueJson))
            {
                valueMap = new Dictionary<string, string>();
                valueMap.Add(key, valueJson);
            }
            else
            {
                valueMap = Json.FromJson<Dictionary<string, string>>(valueJson);
                if (valueMap != null)
                {
                    valueMap[key] = value;
                }
                else
                {
                    valueMap = new Dictionary<string, string>();
                    valueMap.Add(key, valueJson);
                }
            }

            await File.WriteAllTextAsync(localFilePath, Json.ToJson(valueMap));
        }

        private string GetOrCreateFilePath()
        {
            if (_filePath == null)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, local_application_folder_name);
                if (!Directory.Exists(path))
                {
                    _ = Directory.CreateDirectory(path);
                }

                path = Path.Combine(path, local_application_file_name);
                if (!File.Exists(path))
                {
                    _ = File.Create(path);
                }

                _filePath = path;
            }

            return _filePath;
        }
    }
}
