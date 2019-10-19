using System.Collections.Generic;
using Newtonsoft.Json;

namespace GuigleApi.Models.Address
{
    [JsonObject]
    public class AddressT : AddressBase, IAddress
    {
        [JsonIgnore]
        public List<AddressComponent> AddressComponents { get; set; }

        [JsonProperty("address_components")]
        public List<AddressComponentT> AddressComponentsT { get; set; }

        [JsonIgnore]
        public List<AddressComponentS> AddressComponentsS { get; set; }

        [JsonProperty("types")]
        public List<AddressType> Types { get; set; }

        [JsonIgnore]
        public List<string> StringTypes { get; set; }
    }
}
