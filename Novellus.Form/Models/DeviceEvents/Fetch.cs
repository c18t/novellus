namespace Novellus.Models.DeviceEvents
{
    using Newtonsoft.Json;

    [JsonObject("fetchRequest")]
    public class FetchRequest : Request
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    [JsonObject("fetchResponse")]
    public class FetchResponse : Response
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
