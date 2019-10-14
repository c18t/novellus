namespace Novellus.Models.DeviceEvents
{
    using Newtonsoft.Json;

    [JsonObject("request")]
    public class Request
    {
        [JsonProperty("uuid")]
        public string UUID { get; set; }
    }
}
