using System.Linq;
using GuigleApi.Models.Address;
using GuigleApi.Models.Response;

namespace GuigleApi.Models.Extension
{
    public static class ResponseAddressExtension
    {
        public static string GetCountry(this Response<Address.Address> response)
        {
            var addressComponents = response.Results?.SelectMany(t => t.AddressComponents).ToList();

            return addressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.country.ToString()) ?? false)?.LongName;
        }

        public static string GetState(this Response<Address.Address> response)
        {
            var addressComponents = response.Results?.SelectMany(t => t.AddressComponents).ToList();

            return addressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_1.ToString()) ?? false)?.ShortName;
        }


        public static string GetCity(this Response<Address.Address> response)
        {
            var addressComponents = response.Results?.SelectMany(t => t.AddressComponents).ToList();

            return addressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_2.ToString()) ?? false)?.ShortName;
        }

        public static string GetSuburb(this Response<Address.Address> response)
        {
            var addressComponents = response.Results?.SelectMany(t => t.AddressComponents).ToList();

            return addressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_3.ToString()) ?? false)?.ShortName ??
                   addressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.locality.ToString()) ?? false)?.ShortName;
        }
    }
}
