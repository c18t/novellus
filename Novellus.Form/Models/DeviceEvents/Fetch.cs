using Newtonsoft.Json;
using System;

namespace Novellus.Models.DeviceEvents
{
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
