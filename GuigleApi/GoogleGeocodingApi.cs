using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GuigleApi.Models.Address;
using GuigleApi.Models.Response;

namespace GuigleApi
{
    public class GoogleGeocodingApi : GoogleApiBase, IGoogleGeocodingApi
    {
        public GoogleGeocodingApi(string apiKey) : base(apiKey)
        {
        }

        /// <summary>
        /// Gets all addresses returned from Google GeoCode Api based on the coordinates provided.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="lat">The latitude to search on Google Api.</param>
        /// <param name="lng">The longitude to search on Google Api.</param>
        /// <returns>Returns all results from Google Api as an Response<Address>.</returns>
        public async Task<Response<Address>> GetAddressFromCoordinates(HttpClient client, double lat, double lng)
        {
            var location = $"{lat},{lng}";

            var uri = GetGeocodingQueryString(("latlng", location));

            var response = await client.GetAsync(uri);

            return await Address.ParseResponse(response);
        }

        /// <summary>
        /// Gets the city, state and country from a list of address components.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="lat">The latitude to search on Google Api.</param>
        /// <param name="lng">The longitude to search on Google Api.</param>
        /// <returns>Returns a Tuple<string, string, string> where item1 is the city short name, item2 is the state short name and item3 is the country long name. Returns a Tuple containing nulls if nothing is returned from the Api.</returns>
        public async Task<Tuple<string, string, string>> GetCityFromCoordinates(HttpClient client, double lat, double lng)
        {
            var contentResult = await GetAddressFromCoordinates(client, lat, lng);

            var addressComponents = contentResult.Results.SelectMany(t => t.AddressComponents).ToList();

            var city =
                addressComponents.FirstOrDefault(r => r.Types.Contains(AddressType.administrative_area_level_2))?.ShortName ??
                addressComponents.FirstOrDefault(r => r.Types.Contains(AddressType.administrative_area_level_3))?.ShortName ??
                addressComponents.FirstOrDefault(r => r.Types.Contains(AddressType.locality))?.ShortName;

            var state = addressComponents.FirstOrDefault(r => r.Types.Contains(AddressType.administrative_area_level_1))?.ShortName;

            var country = addressComponents.FirstOrDefault(r => r.Types.Contains(AddressType.country))?.LongName;

            return new Tuple<string, string, string>(city, state, country);
        }

        /// <summary>
        /// Search for an address and returns all results from Google Api.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="address">The address to search on Google Api.</param>
        /// <returns>Returns all results from Google Api as an Response<Address>.</returns>
        public async Task<Response<Address>> SearchAddress(HttpClient client, string address)
        {
            var uri = GetGeocodingQueryString(("address", address));

            var response = await client.GetAsync(uri);

            return await Address.ParseResponse(response);
        }

        /// <summary>
        /// Gets the first coordinates from a possible list of address components.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="address">The address to search on Google Api.</param>
        /// <returns>Returns a Tuple<double, double> where item1 is latitude and item2 is longitude. Returns null if nothing is returned from the Api.</returns>
        public async Task<Tuple<double, double>> GetCoordinatesFromAddress(HttpClient client, string address)
        {
            var result = await SearchAddress(client, address);

            var firstResult = result.Results.FirstOrDefault();

            if (firstResult != null)
            {
                return new Tuple<double, double>(
                    firstResult.Geometry?.Location?.Lat ?? 0,
                    firstResult.Geometry?.Location?.Lng ?? 0);
            }

            return null;
        }

        /// <summary>
        /// Search for an address preferring results within the viewport provided and returns all results from Google Api.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="address">The address to search on Google Api.</param>
        /// <param name="southwest">The south west coordinates of the bounding box.</param>
        /// <param name="northeast">The north east coordinates of the bounding box.</param>
        /// <returns>Returns all results from Google Api as an Response<Address>.</returns>
        public async Task<Response<Address>> SearchAddress(HttpClient client, string address, Tuple<double, double> southwest, Tuple<double, double> northeast)
        {
            var uri = GetGeocodingQueryString(
                ("address", address),
                ("bounds", $"{southwest.Item1},{southwest.Item2}|{northeast.Item1},{northeast.Item2}"));

            var response = await client.GetAsync(uri);

            return await Address.ParseResponse(response);
        }
    }
}
