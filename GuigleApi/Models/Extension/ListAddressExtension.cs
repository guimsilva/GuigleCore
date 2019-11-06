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
                addresses?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.country.ToString()) ?? false) ??
                addresses?.FirstOrDefault(r => r.AddressComponents?.Exists(a => a.StringTypes?.Contains(AddressType.country.ToString()) ?? false) ?? false);
        }

        public static Address.Address GetState(this List<Address.Address> addresses)
        {
            return
                addresses?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_1.ToString()) ?? false) ??
                addresses?.FirstOrDefault(r => r.AddressComponents?.Exists(a => a.StringTypes?.Contains(AddressType.administrative_area_level_1.ToString()) ?? false) ?? false);
        }

        public static Address.Address GetCity(this List<Address.Address> addresses)
        {
            return
                addresses?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_2.ToString()) ?? false) ??
                addresses?.FirstOrDefault(r => r.AddressComponents?.Exists(a => a.StringTypes?.Contains(AddressType.administrative_area_level_2.ToString()) ?? false) ?? false);
        }

        public static Address.Address GetSuburb(this List<Address.Address> addresses)
        {
            return
                addresses?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_3.ToString()) ?? false) ??
                addresses?.FirstOrDefault(r => r.AddressComponents?.Exists(a => a.StringTypes?.Contains(AddressType.administrative_area_level_3.ToString()) ?? false) ?? false) ??
                addresses?.FirstOrDefault(r =>
                    r.AddressComponents != null && r.AddressComponents.Any(a =>
                        a.StringTypes != null && a.StringTypes.Any()
                        && a.StringTypes.Contains(AddressType.locality.ToString())
                        && a.StringTypes.Contains(AddressType.political.ToString())));
        }
    }
}
