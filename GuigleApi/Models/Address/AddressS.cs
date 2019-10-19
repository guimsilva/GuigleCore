using System.Collections.Generic;
using Newtonsoft.Json;

namespace GuigleApi.Models.Address
{
    [JsonObject]
    public class AddressS : AddressBase, IAddress
    {
        [JsonIgnore]
        public List<AddressComponent> AddressComponents { get; set; }

        [JsonIgnore]
        public List<AddressComponentT> AddressComponentsT { get; set; }

        [JsonProperty("address_components")]
        public List<AddressComponentS> AddressComponentsS { get; set; }

        [JsonIgnore]
        public List<AddressType> Types { get; set; }

        [JsonProperty("types")]
        public List<string> StringTypes { get; set; }
    }
}
