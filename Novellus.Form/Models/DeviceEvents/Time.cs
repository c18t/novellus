using Newtonsoft.Json;
using System;

namespace Novellus.Models.DeviceEvents
{
    [JsonObject("timeRequest")]
    public class TimeRequest : Request
    {
    }

    [JsonObject("timeResponse")]
    public class TimeResponse : Response
    {
        [JsonProperty("time")]
        public DateTime Time { get; set; }
    }
}
