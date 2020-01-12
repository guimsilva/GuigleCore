using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GuigleApi.Models.Response;
using Newtonsoft.Json;

namespace GuigleApi.Models.Address
{
    public class Address : IAddress
    {
        public string PlaceId { get; set; }

        public List<AddressComponent> AddressComponents { get; set; }

        public List<AddressComponentT> AddressComponentsT { get; set; }

        public List<AddressComponentS> AddressComponentsS { get; set; }

        public string FormattedAddress { get; set; }

        public Geometry Geometry { get; set; }

        public List<AddressType> Types { get; set; }

        public List<string> StringTypes { get; set; }

        public string CountryShortName =>
            AddressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.country.ToString()) ?? false)?.ShortName;
        public string CountryLongName =>
            AddressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.country.ToString()) ?? false)?.LongName;

        public string StateShortName =>
            AddressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_1.ToString()) ?? false)?.ShortName;
        public string StateLongName =>
            AddressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_1.ToString()) ?? false)?.LongName;
        
        public string CityShortName =>
            AddressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_2.ToString()) ?? false)?.ShortName;
        public string CityLongName =>
            AddressComponents?
                .FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_2.ToString()) ?? false)?.LongName;

        public string SuburbShortName =>
            AddressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_3.ToString()) ?? false)?.ShortName ??
            AddressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.locality.ToString()) ?? false)?.ShortName ??
            AddressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.sublocality.ToString()) ?? false)?.ShortName ??
            AddressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.sublocality_level_1.ToString()) ?? false)?.ShortName;
        public string SuburbLongName =>
            AddressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.administrative_area_level_3.ToString()) ?? false)?.LongName ??
            AddressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.locality.ToString()) ?? false)?.LongName ??
            AddressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.sublocality.ToString()) ?? false)?.LongName ??
            AddressComponents?.FirstOrDefault(r => r.StringTypes?.Contains(AddressType.sublocality_level_1.ToString()) ?? false)?.LongName;

        public static async Task<Response<Address>> ParseResponse(HttpResponseMessage response)
        {
            if (response is null)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw await Response<AddressT>.ResponseError(response);
            }

            var content = await response.Content.ReadAsStringAsync();

            Response<Address> result;
            try
            {
                result = ParseResponse(JsonConvert.DeserializeObject<Response<AddressT>>(content));
                result.Results?.ForEach(SetAddressStringTypes);
                result.Candidates?.ForEach(SetAddressStringTypes);
            }
            catch (JsonSerializationException e)
            {
                result = ParseResponse(JsonConvert.DeserializeObject<Response<AddressS>>(content));
            }

            if (result is null)
            {
                throw await Response<AddressT>.ResponseError(response);
            }

            if (result.Status == "INVALID_REQUEST")
            {
                throw new HttpRequestException(result.ErrorMessage ?? "Error");
            }

            return result;
        }

        private static void SetAddressStringTypes(Address address)
        {
            if (address.Types is null) return;
            address.StringTypes = new List<string>();
            address.Types.ForEach(type => address.StringTypes.Add(type.ToString()));
            address.AddressComponents.ForEach(component =>
            {
                component.StringTypes = new List<string>();
                component.Types.ForEach(type => component.StringTypes.Add(type.ToString()));
            });
        }

        private static Response<Address> ParseResponse(Response<AddressT> addressT)
        {
            if (addressT is null)
            {
                return null;
            }

            return new Response<Address>()
            {
                Candidates = addressT.Candidates?.Select(ParseAddress).ToList(),
                ErrorMessage = addressT.ErrorMessage,
                HtmlAttributions = addressT.HtmlAttributions,
                NextPageToken = addressT.NextPageToken,
                Result = ParseAddress(addressT.Result),
                Results = addressT.Results?.Select(ParseAddress).ToList(),
                Status = addressT.Status
            };
        }

        private static Response<Address> ParseResponse(Response<AddressS> addressS)
        {
            if (addressS is null)
            {
                return null;
            }

            return new Response<Address>()
            {
                Candidates = addressS.Candidates?.Select(ParseAddress).ToList(),
                ErrorMessage = addressS.ErrorMessage,
                HtmlAttributions = addressS.HtmlAttributions,
                NextPageToken = addressS.NextPageToken,
                Result = ParseAddress(addressS.Result),
                Results = addressS.Results?.Select(ParseAddress).ToList(),
                Status = addressS.Status
            };
        }

        public static Address ParseAddress(IAddress address)
        {
            if (address is null)
            {
                return null;
            }

            return new Address()
            {
                PlaceId = address.PlaceId,
                AddressComponents =
                    address.AddressComponentsT?.Select(AddressComponent.Parse).ToList() ??
                    address.AddressComponentsS?.Select(AddressComponent.Parse).ToList(),
                AddressComponentsT = address.AddressComponentsT,
                AddressComponentsS = address.AddressComponentsS,
                FormattedAddress = address.FormattedAddress,
                Geometry = address.Geometry,
                StringTypes = address.StringTypes,
                Types = address.Types
            };
        }
    }
}
