using Newtonsoft.Json;
using System.Collections.Generic;

namespace GuigleApi.Models.Place
{
    [JsonObject]
    public class PlaceS : PlaceBase, IPlace
    {
        public List<PlaceType> Types { get; set; }

        [JsonProperty("types")]
        public List<string> StringTypes { get; set; }
    }
}
