using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GuigleApi.Models.Place;
using GuigleApi.Models.Response;

namespace GuigleApi
{
    public class GooglePlacesApi : GoogleApiBase, IGooglePlacesApi
    {
        public GooglePlacesApi(string apiKey) : base(apiKey)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="query"></param>
        /// <param name="region">https://en.wikipedia.org/wiki/List_of_Internet_top-level_domains#Country_code_top-level_domains</param>
        /// <param name="returnFields"></param>
        /// <returns></returns>
        public async Task<Response<Place>> FindBusiness(HttpClient client, string query, double? lat = null, double? lng = null, int? radiusInMeters = 50000, string region = null, string language = null, string pageToken = null, PlaceType? type = null, string[] returnFields = null)
        {
            var location = lat.HasValue && lng.HasValue ? $"{lat},{lng}" : string.Empty;

            var uri = GetPlacesQueryString(
                "textsearch",
                ("query", query),
                ("type", type?.ToString()),
                ("location", location),
                ("radius", radiusInMeters?.ToString()),
                ("region", region),
                ("language", language),
                ("pageToken", pageToken),
                ("fields", string.Join(",", returnFields ?? GoogleApiBase.SearchFieldsBasic.Concat(GoogleApiBase.SearchFieldsContact).Concat(GoogleApiBase.SearchFieldsAtmosphere)))
            );

            var response = await client.GetAsync(uri);

            return await Place.ParseResponse(response);
        }

        public async Task<Response<Place>> FindPlaces(HttpClient client, string input, string[] returnFields = null)
        {
            var uri = GetPlacesQueryString(
                "findplacefromtext",
                ("input", input),
                ("inputtype", "textquery"),
                ("fields", string.Join(",", returnFields ?? GoogleApiBase.SearchFieldsBasic.Concat(GoogleApiBase.SearchFieldsContact).Concat(GoogleApiBase.SearchFieldsAtmosphere)))
            );

            var response = await client.GetAsync(uri);

            return await Place.ParseResponse(response);
        }

        public async Task<Response<Place>> GetPlaceById(HttpClient client, string placeId, string[] returnFields = null)
        {
            var uri = GetPlacesQueryString(
                "details",
                ("place_id", placeId),
                ("fields", string.Join(",", returnFields ?? GoogleApiBase.DetailsFieldsBasic.Concat(GoogleApiBase.DetailsFieldsContact).Concat(GoogleApiBase.DetailsFieldsAtmosphere)))
            );

            var response = await client.GetAsync(uri);

            return await Place.ParseResponse(response);
        }

        /// <summary>
        /// Gets up to 20 places returned from Google Places Api based on the coordinates provided.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="lat">The latitude to search on Google Api.</param>
        /// <param name="lng">The longitude to search on Google Api.</param>
        /// <param name="radiusInMeters">The maximum distance to search. Narrow this value down to get fewer and more accurate results.</param>
        /// <param name="language">See https://developers.google.com/maps/faq?authuser=1#languagesupport.</param>
        /// <param name="type">The type of the place. E.g. restaurant.</param>
        /// <param name="keyWord">Any key word to search for. E.g. cruise.</param>
        /// <param name="rankBy">Rank by distance or prominence.</param>
        /// <param name="moreOptionalParameters">There are more optional parameters that can be added to the search request. Check Google Developers Api for more info.</param>
        public async Task<Response<Place>> SearchPlaceNearBy(HttpClient client, double lat, double lng, int? radiusInMeters = 50000, string language = null, PlaceType? type = null, string keyWord = null, RankBy? rankBy = null, string moreOptionalParameters = null)
        {
            ValidateSearchOptionalParams(radiusInMeters, language, type, keyWord, rankBy, moreOptionalParameters);

            var uri = GetPlacesQueryString(
                "nearbysearch",
                ("location", $"{lat},{lng}"),
                ("radius", radiusInMeters.ToString()),
                ("language", language),
                ("type", type?.ToString()),
                ("keyword", keyWord),
                ("rankby", rankBy?.ToString()),
                (string.Empty, moreOptionalParameters)
            );

            var response = await client.GetAsync(uri);

            return await Place.ParseResponse(response);
        }

        /// <summary>
        /// Gets up to 20 places returned from Google Places Api based on the query provided.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="query">The query to be used to search on Google Api.</param>
        /// <param name="lat">The latitude to search on Google Api.</param>
        /// <param name="lng">The longitude to search on Google Api.</param>
        /// <param name="radiusInMeters">The maximum distance to search. Narrow this value down to get fewer and more accurate results.</param>
        /// <param name="language">See https://developers.google.com/maps/faq?authuser=1#languagesupport.</param>
        /// <param name="type"></param>
        /// <param name="moreOptionalParameters">There are more optional parameters that can be added to the search request. Check Google Developers Api for more info.</param>
        public async Task<Response<Place>> SearchPlaceByQuery(HttpClient client, string query, double? lat, double? lng, int? radiusInMeters = null, string language = null, PlaceType? type = null, string moreOptionalParameters = null)
        {
            ValidateSearchOptionalParams(radiusInMeters, language, type, null, null, moreOptionalParameters);

            var location = lat.HasValue && lng.HasValue ? $"{lat},{lng}" : string.Empty;

            var uri = GetPlacesQueryString(
                "textsearch",
                ("query", query),
                ("location", location),
                ("radius", radiusInMeters?.ToString()),
                ("language", language),
                ("type", type?.ToString()),
                (string.Empty, moreOptionalParameters)
            );

            var response = await client.GetAsync(uri);

            return await Place.ParseResponse(response);
        }

        /// <summary>
        /// Gets next page (or next 20 places) returned from Google Places Api based on the token provided.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="pageToken">The token returned on a previous search. This token will be used to retrieve the next page (20 results).</param>
        /// <returns></returns>
        public async Task<Response<Place>> SearchPlaceNearBy(HttpClient client, string pageToken)
        {
            var uri = new Uri(string.Format($"{GoogleApiBase.GeoPlacesUrl}nearbysearch/json?pagetoken={pageToken}&key={GoogleApiKey}", string.Empty));

            var response = await client.GetAsync(uri);

            return await Place.ParseResponse(response);
        }
    }
}