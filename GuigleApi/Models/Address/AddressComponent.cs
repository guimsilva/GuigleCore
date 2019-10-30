using System.Collections.Generic;

namespace GuigleApi.Models.Address
{
    public class AddressComponent : IAddressComponent
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public List<AddressType> Types { get; set; }
        public List<string> StringTypes { get; set; }

        public static AddressComponent Parse(AddressComponentT addressComponentT)
        {
            if (addressComponentT is null)
            {
                return null;
            }

            return new AddressComponent()
            {
                LongName = addressComponentT.LongName,
                ShortName = addressComponentT.ShortName,
                Types = addressComponentT.Types,
                StringTypes = addressComponentT.StringTypes
            };
        }

        public static AddressComponent Parse(AddressComponentS addressComponentS)
        {
            if (addressComponentS is null)
            {
                return null;
            }

            return new AddressComponent()
            {
                LongName = addressComponentS.LongName,
                ShortName = addressComponentS.ShortName,
                Types = addressComponentS.Types,
                StringTypes = addressComponentS.StringTypes
            };
        }
    }
}
