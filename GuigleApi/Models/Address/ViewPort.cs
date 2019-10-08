using Newtonsoft.Json;

namespace GuigleApi.Models.Address
{
    [JsonObject]
    public class ViewPort
    {
        [JsonProperty("northeast")]
        public Location Northeast { get; set; }

        [JsonProperty("southwest")]
        public Location Southwest { get; set; }
    }
}
