using Newtonsoft.Json;
using System;

namespace Novellus.Models.DeviceEvents
{
    [JsonObject("alertRequest")]
    public class AlertRequest : Request
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    [JsonObject("alertResponse")]
    public class AlertResponse : Response
    {
    }
}
