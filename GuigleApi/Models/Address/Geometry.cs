using Newtonsoft.Json;

namespace GuigleApi.Models.Address
{
    [JsonObject]
    public class Geometry
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        [JsonProperty("viewport")]
        public ViewPort ViewPort { get; set; }
    }
}
