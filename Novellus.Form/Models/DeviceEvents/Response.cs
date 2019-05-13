using Newtonsoft.Json;

namespace Novellus.Models.DeviceEvents
{
    [JsonObject("response")]
    public class Response
    {
        [JsonProperty("uuid")]
        public string UUID { get; set; }
    }
}
