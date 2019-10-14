namespace Novellus.Models.DeviceEvents
{
    using System;
    using Newtonsoft.Json;

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
