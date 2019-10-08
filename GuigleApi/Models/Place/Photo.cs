using System.Collections.Generic;
using Newtonsoft.Json;

namespace GuigleApi.Models.Place
{
    public class Photo
    {
        [JsonProperty("photo_reference")]
        public string PhotoReference { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("html_attributions")]
        public List<string> HtmlAttributions { get; set; }
    }
}
