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
        public DateTime time { get; set; }
    }
}
