using Newtonsoft.Json;
using System;

namespace Novellus.Models.DeviceEvents
{
    [JsonObject("alertRequest")]
    public class AlertRequest : Request
    {
        public string message { get; set; }
    }

    [JsonObject("alertResponse")]
    public class AlertResponse : Response
    {
    }
}
