using System;
using Newtonsoft.Json;

namespace Novellus.Models
{
    [JsonObject]
    public class ActionEvent
    {
        [JsonProperty("action", Required = Required.Always)]
        public string Action { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
