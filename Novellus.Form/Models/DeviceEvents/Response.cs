namespace Novellus.Models.DeviceEvents
{
    using Newtonsoft.Json;

    [JsonObject("response")]
    public class Response
    {
        [JsonProperty("uuid")]
        public string UUID { get; set; }
    }
}
