using System.Collections.Generic;
using Newtonsoft.Json;

namespace GuigleApi.Models.Address
{
    [JsonObject]
    public class AddressComponentS : AddressComponentBase, IAddressComponent
    {
        public List<AddressType> Types { get; set; }

        [JsonProperty("types")]
        public List<string> StringTypes { get; set; }
    }
}
