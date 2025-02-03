using System.Text.Json;

namespace WebStub.Core
{
    public class Json
    {
        public static string ToJson(object obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj);

            }
            catch (Exception e)
            {
                Logger.Warn("Faild Json Serialize. " + e.Message);
                return "";
            }
        }

        public static T? FromJson<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception e)
            {
                Logger.Warn("Faild Json Deserialize. (json : " + json + ", message : " + e.Message + ")");
                return default;
            }
        }
    }
}
