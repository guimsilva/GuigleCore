using Newtonsoft.Json;

namespace GuigleApi.Models.Place
{
    public class OpeningHours
    {
        [JsonProperty("open_now")]
        public bool OpenNow { get; set; }
    }
}
