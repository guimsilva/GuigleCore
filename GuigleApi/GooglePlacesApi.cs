﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GuigleApi.Models.Place;
using GuigleApi.Models.Response;

namespace GuigleApi
{
    public class GooglePlacesApi : GoogleApiBase, IGooglePlacesApi
    {
        private readonly GoogleGeocodingApi _googleGeocodingApi;
        public int MaxAttempsForPageTokenRequests { get; set; }
        private readonly List<PlaceType> _politicalLocalityTypes = new List<PlaceType>() { PlaceType.locality, PlaceType.political };

        public GooglePlacesApi(string apiKey, int maxAttempsForPageTokenRequests = 5) : base(apiKey)
        {
            _googleGeocodingApi = new GoogleGeocodingApi(apiKey);
            MaxAttempsForPageTokenRequests = maxAttempsForPageTokenRequests;
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

            var uri = "";
            if (string.IsNullOrWhiteSpace(pageToken))
            {
                uri = GetPlacesQueryString(
                    "textsearch",
                    ("query", query),
                    ("type", type?.ToString()),
                    ("location", location),
                    ("radius", radiusInMeters?.ToString()),
                    ("region", region),
                    ("language", language),
                    ("fields", string.Join(",", returnFields ?? GoogleApiBase.SearchFieldsBasic.Concat(GoogleApiBase.SearchFieldsContact).Concat(GoogleApiBase.SearchFieldsAtmosphere)))
                );
            }
            else
            {
                uri = $"{GeoPlacesUrl}textsearch/json?pagetoken={pageToken}&key={GoogleApiKey}";
            }

            return await MakeRequest(client, uri);
        }

        /// <summary>
        /// WARNING: This may make several additional calls to Geocoding Api.
        /// Gets up to 20 business and their address returned from Google Places and Geocoding Apis,
        /// based on the coordinates provided and using GetPlaceDetailsById to get the formatted address
        /// and SearchAddress to return only one address.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="query"></param>
        /// <param name="region">https://en.wikipedia.org/wiki/List_of_Internet_top-level_domains#Country_code_top-level_domains</param>
        /// <param name="returnFields"></param>
        /// <returns></returns>
        public async Task<List<Place>> FindBusinessAddress(HttpClient client, string query, double? lat = null, double? lng = null, int? radiusInMeters = 50000, string region = null, string language = null, string pageToken = null, PlaceType? type = null, string[] returnFields = null)
        {
            var businessResult = await FindBusiness(client, query, lat, lng, radiusInMeters, region, language, pageToken, type, returnFields);

            var businessAddressTasks = businessResult.Results.Select(async business =>
            {
                var placeDetailsResponse = await GetPlaceDetailsById(client, business.PlaceId, new[] { "formatted_address" });
                var addressesResponse = await _googleGeocodingApi.SearchAddress(client, placeDetailsResponse.Result.FormattedAddress);
                business.Addresses = addressesResponse.Results;
                return business;
            });

            return (await Task.WhenAll(businessAddressTasks)).ToList();
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

        public async Task<Response<Place>> GetPlaceDetailsById(HttpClient client, string placeId, string[] returnFields = null, string sessionToken = null)
        {
            var uri = GetPlacesQueryString(
                "details",
                ("place_id", placeId),
                ("sessiontoken", sessionToken),
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
        public async Task<Response<Place>> SearchPlaceNearBy(HttpClient client, double lat, double lng, int? radiusInMeters = null, string language = null, PlaceType? type = null, string keyWord = null, RankBy? rankBy = null, params (string, string)[] moreOptionalParameters)
        {
            if (!radiusInMeters.HasValue && !rankBy.HasValue)
            {
                throw new ArgumentException("Either Rankby.distance or radius is required'");
            }

            ValidateSearchOptionalParams(radiusInMeters, language, type, keyWord, rankBy, moreOptionalParameters);

            var uri = GetPlacesQueryString(
                "nearbysearch",
                new (string, string)[] {
                ("location", $"{lat},{lng}"),
                ("radius", radiusInMeters.ToString()),
                ("language", language),
                ("type", type?.ToString()),
                ("keyword", keyWord),
                ("rankby", rankBy?.ToString())}
                    .Concat(moreOptionalParameters).ToArray()
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
        public async Task<Response<Place>> SearchPlaceByQuery(HttpClient client, string query, double? lat, double? lng, int? radiusInMeters = null, string language = null, PlaceType? type = null, params (string, string)[] moreOptionalParameters)
        {
            ValidateSearchOptionalParams(radiusInMeters, language, type, null, null, moreOptionalParameters);

            var location = lat.HasValue && lng.HasValue ? $"{lat},{lng}" : string.Empty;

            var uri = "";
            if (!(moreOptionalParameters?.ToList()?.Exists(p => p.Item1 == "pagetoken") ?? false))
            {
                uri = GetPlacesQueryString(
                    "textsearch",
                    new (string, string)[] {("query", query),
                    ("location", location),
                    ("radius", radiusInMeters?.ToString()),
                    ("language", language),
                    ("type", type?.ToString())}
                        .Concat(moreOptionalParameters).ToArray()
                );
            }
            else
            {
                var pageToken = moreOptionalParameters.First(p => p.Item1 == "pagetoken").Item2;
                uri = $"{GeoPlacesUrl}textsearch/json?pagetoken={pageToken}&key={GoogleApiKey}";
            }

            return await MakeRequest(client, uri);
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

        /// <summary>
        /// Similar to the "What's here?" in Google Maps.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="address">The address to search on Google Api.</param>
        /// <param name="lat">The latitude to search on Google Api.</param>
        /// <param name="lng">The longitude to search on Google Api.</param>
        public async Task<Place> GetExactPlaceByLocation(HttpClient client, double lat, double lng)
        {
            var place = await SearchPlaceNearBy(client, lat, lng, type: PlaceType.point_of_interest, rankBy: RankBy.prominence, radiusInMeters: 10);

            var exactPlace =
                place.Results
                    .FirstOrDefault(result =>
                        !_politicalLocalityTypes.All(pl =>
                            (result.Types?.Contains(pl) ?? false)
                            || (result.StringTypes?.Contains(pl.ToString()) ?? false)));

            if (exactPlace is null)
            {
                place = await SearchPlaceNearBy(client, lat, lng, 5);

                exactPlace =
                    place.Results
                        .FirstOrDefault(result =>
                            !_politicalLocalityTypes.All(pl =>
                                (result.Types?.Contains(pl) ?? false)
                                || (result.StringTypes?.Contains(pl.ToString()) ?? false)))
                    ?? place.Results.FirstOrDefault();
            }

            return exactPlace;
        }

        /// <summary>
        /// Similar to the "What's here?" in Google Maps.
        /// </summary>
        /// <param name="client">The HttpClient object. Make sure it's not passed closed.</param>
        /// <param name="address">The address to search on Google Api.</param>
        public async Task<Place> GetExactPlaceByAddress(HttpClient client, string address)
        {
            var location = await _googleGeocodingApi.GetCoordinatesFromAddress(client, address);

            var exactPlace = await GetExactPlaceByLocation(client, location.Lat, location.Lng);

            return exactPlace;
        }

        /// <summary>
        /// WARNING: This may make several additional calls to Geocoding Api.
        /// Gets up to 20 places and their addresses returned from Google Places and Geocoding Apis,
        /// based on the coordinates provided.
        /// </summary>
        public async Task<List<Place>> SearchPlaceAddressesNearBy(HttpClient client, double lat, double lng, int? radiusInMeters = null, PlaceType? type = null, RankBy? rankBy = null)
        {
            var placeResult = await SearchPlaceNearBy(client, lat, lng, radiusInMeters, type: type, rankBy: rankBy);

            var placeAddressTasks = placeResult.Results.Select(async place =>
            {
                var result = await _googleGeocodingApi.GetAddressFromCoordinates(client, place.Geometry.Location.Lat, place.Geometry.Location.Lng);
                place.Addresses = result.Results;
                return place;
            });

            return (await Task.WhenAll(placeAddressTasks)).ToList();
        }

        /// <summary>
        /// WARNING: This may make several additional calls to Geocoding Api.
        /// Gets up to 20 places and their address returned from Google Places and Geocoding Apis,
        /// based on the coordinates provided and using GetPlaceDetailsById to get the formatted address
        /// and SearchAddress to return only one address.
        /// </summary>
        public async Task<List<Place>> SearchPlaceAddressNearBy(HttpClient client, double lat, double lng, int? radiusInMeters = null, PlaceType? type = null, RankBy? rankBy = null)
        {
            var placeResult = await SearchPlaceNearBy(client, lat, lng, radiusInMeters, type: type, rankBy: rankBy);

            var placeAddressTasks = placeResult.Results.Select(async place =>
            {
                var placeDetailsResponse = await GetPlaceDetailsById(client, place.PlaceId, new []{ "formatted_address" });
                var addressesResponse = await _googleGeocodingApi.SearchAddress(client, placeDetailsResponse.Result.FormattedAddress);
                place.Addresses = addressesResponse.Results;
                return place;
            });

            return (await Task.WhenAll(placeAddressTasks)).ToList();
        }

        private async Task<Response<Place>> MakeRequest(HttpClient client, string uri)
        {
            var hasPageToken = uri.Contains("pagetoken");
            var maxAttempts = hasPageToken ? MaxAttempsForPageTokenRequests : 1;
            var attempts = 0;

            Response<Place> result = null;
            while (attempts < maxAttempts)
            {
                try
                {
                    var response = await client.GetAsync(uri);
                    result = await Place.ParseResponse(response);
                    break;
                }
                catch (Exception e)
                {
                    if (!hasPageToken || e.Message != "INVALID_REQUEST")
                    {
                        throw;
                    }
                    attempts++;
                    await Task.Delay(300);
                }
            }

            return result;
        }
    }
}
