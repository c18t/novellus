namespace Novellus.Models.DeviceEvents
{
    using Newtonsoft.Json;

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
