using System.Collections.Generic;
using System.Linq;
using GuigleApi.Models.Address;

namespace GuigleApi.Models.Extension
{
    public static class ListAddressExtension
    {
        public static Address.Address GetCountry(this List<Address.Address> addresses)
        {
            return
                addresses?.Count == 1 ? addresses.First() :
                addresses?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.country.ToString()) ?? false);
        }

        public static Address.Address GetState(this List<Address.Address> addresses)
        {
            return
                addresses?.Count == 1 ? addresses.First() :
                addresses?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_1.ToString()) ?? false);
        }

        public static Address.Address GetCity(this List<Address.Address> addresses)
        {
            return
                addresses?.Count == 1 ? addresses.First() :
                addresses?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_2.ToString()) ?? false);
        }

        public static Address.Address GetSuburb(this List<Address.Address> addresses)
        {
            return
                addresses?.Count == 1 ? addresses.First() :
                addresses?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_3.ToString()) ?? false) ??
                addresses?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.locality.ToString()) ?? false);
        }
    }
}
