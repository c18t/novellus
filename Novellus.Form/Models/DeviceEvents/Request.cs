using Newtonsoft.Json;

namespace Novellus.Models.DeviceEvents
{
    [JsonObject("request")]
    public class Request
    {
        [JsonProperty("uuid")]
        public string UUID { get; set; }
    }
}
