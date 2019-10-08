using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GuigleApi.Models.Place
{
    [JsonObject]
    public class PlaceT : PlaceBase, IPlace
    {
        [JsonProperty("types")]
        public List<PlaceType> Types { get; set; }

        public List<string> StringTypes { get; set; }
    }
}
