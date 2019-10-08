using System.Collections.Generic;
using Newtonsoft.Json;

namespace GuigleApi.Models.Address
{
    [JsonObject]
    public class AddressComponentT : AddressComponentBase, IAddressComponent
    {
        /// <summary>
        /// From enum AddressType
        /// </summary>
        [JsonProperty("types")]
        public List<AddressType> Types { get; set; }

        public List<string> StringTypes { get; set; }
    }
}
