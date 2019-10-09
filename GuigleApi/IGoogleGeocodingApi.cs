using System;
using System.Net.Http;
using System.Threading.Tasks;
using GuigleApi.Models.Address;
using GuigleApi.Models.Response;

namespace GuigleApi
{
    public interface IGoogleGeocodingApi
    {
        /// <summary>
        /// Gets all addresses returned from Google GeoCode Api based on the coordinates provided.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="lat">The latitude to search on Google Api.</param>
        /// <param name="lng">The longitude to search on Google Api.</param>
        /// <returns>Returns all results from Google Api as an Response<Address>.</returns>
        Task<Response<Address>> GetAddressFromCoordinates(HttpClient client, double lat, double lng);

        /// <summary>
        /// Gets the city, state and country from a list of address components.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="lat">The latitude to search on Google Api.</param>
        /// <param name="lng">The longitude to search on Google Api.</param>
        /// <returns>Returns a Tuple<string, string, string> where item1 is the city short name, item2 is the state short name and item3 is the country long name. Returns a Tuple containing nulls if nothing is returned from the Api.</returns>
        Task<Tuple<string, string, string>> GetCityFromCoordinates(HttpClient client, double lat, double lng);

        /// <summary>
        /// Search for an address and returns all results from Google Api.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="address">The address to search on Google Api.</param>
        /// <returns>Returns all results from Google Api as an Response<Address>.</returns>
        Task<Response<Address>> SearchAddress(HttpClient client, string address);

        /// <summary>
        /// Gets the first coordinates from a possible list of address components.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="address">The address to search on Google Api.</param>
        /// <returns>Returns a Tuple<double, double> where item1 is latitude and item2 is longitude. Returns null if nothing is returned from the Api.</returns>
        Task<Tuple<double, double>> GetCoordinatesFromAddress(HttpClient client, string address);

        /// <summary>
        /// Search for an address preferring results within the viewport provided and returns all results from Google Api.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="address">The address to search on Google Api.</param>
        /// <param name="southwest">The south west coordinates of the bounding box.</param>
        /// <param name="northeast">The north east coordinates of the bounding box.</param>
        /// <returns>Returns all results from Google Api as an Response<Address>.</returns>
        Task<Response<Address>> SearchAddress(HttpClient client, string address, Tuple<double, double> southwest, Tuple<double, double> northeast);
    }
}