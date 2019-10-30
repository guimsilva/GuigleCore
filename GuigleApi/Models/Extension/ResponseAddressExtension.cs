using System;
using System.Linq;
using GuigleApi.Models.Address;
using GuigleApi.Models.Response;

namespace GuigleApi.Models.Extension
{
    [Obsolete]
    public static class ResponseAddressExtension
    {
        [Obsolete]
        public static (string, string)? GetCountry(this Response<Address.Address> response)
        {
            var addressComponents = response.Results?.SelectMany(t => t.AddressComponents).ToList();

            var country = addressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.country.ToString()) ?? false);

            if (country is null) return null;

            return (country.ShortName, country.LongName);
        }

        [Obsolete]
        public static (string, string)? GetState(this Response<Address.Address> response)
        {
            var addressComponents = response.Results?.SelectMany(t => t.AddressComponents).ToList();

            var state = addressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_1.ToString()) ?? false);

            if (state is null) return null;

            return (state.ShortName, state.LongName);
        }

        [Obsolete]
        public static string GetCity(this Response<Address.Address> response)
        {
            var addressComponents = response.Results?.SelectMany(t => t.AddressComponents).ToList();

            return addressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_2.ToString()) ?? false)?.ShortName;
        }

        [Obsolete]
        public static AddressComponent GetCityAddressComponent(this Response<Address.Address> response)
        {
            var addressComponents = response.Results?.SelectMany(t => t.AddressComponents).ToList();

            return addressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_2.ToString()) ?? false);
        }

        [Obsolete]
        public static (string, string)? GetSuburb(this Response<Address.Address> response)
        {
            var addressComponents = response.Results?.SelectMany(t => t.AddressComponents).ToList();

            var suburb = addressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_3.ToString()) ?? false) ??
                   addressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.locality.ToString()) ?? false);

            if (suburb is null) return null;

            return (suburb.ShortName, suburb.LongName);
        }

        [Obsolete]
        public static AddressComponent GetSuburbAddressComponent(this Response<Address.Address> response)
        {
            var addressComponents = response.Results?.SelectMany(t => t.AddressComponents).ToList();

            return addressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_3.ToString()) ?? false) ??
                   addressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.locality.ToString()) ?? false);
        }

    }
}
