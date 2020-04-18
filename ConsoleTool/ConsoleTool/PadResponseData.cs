using Newtonsoft.Json;
using System.Collections.Generic;

namespace ConsoleTool
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PadResponseData
    {
        public PadResponseData()
        {
            Success = true;
            Data = new Dictionary<string, object>();
        }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public virtual Dictionary<string, object> Data { get; set; }

        public T GetValue<T>(string key)
        {
            try
            {
                if (Data != null && Data.ContainsKey(key))
                {
                    return (T)Data[key];
                }
            }
            catch
            {
            }
            return default;
        }

        public void SetValue<T>(string key, T value)
        {
            if (Data != null)
            {
                Data[key] = value;
            }
        }

    }
}
