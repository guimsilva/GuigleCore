using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using GuigleApi.Models.Place;

namespace GuigleApi
{
    public class GoogleApiBase
    {
        public static string GeoPlacesUrl { get; set; } = "https://maps.googleapis.com/maps/api/place/";
        public static string GeocodingUrl { get; set; } = "https://maps.googleapis.com/maps/api/geocode/";
        public static string GoogleApiKey { get; set; }

        public static string[] SearchFieldsBasic { get; set; } = { "formatted_address", "geometry", "icon", "name", "permanently_closed", "photos", "place_id", "plus_code", "types" };
        public static string[] SearchFieldsContact { get; set; } = { "opening_hours" };
        public static string[] SearchFieldsAtmosphere { get; set; } = { "price_level", "rating", "user_ratings_total" };

        public static string[] DetailsFieldsBasic { get; set; } = { "address_component", "adr_address", "formatted_address", "geometry", "icon", "name", "permanently_closed", "photo", "place_id", "plus_code", "type", "url", "utc_offset", "vicinity" };
        public static string[] DetailsFieldsContact { get; set; } = { "formatted_phone_number", "international_phone_number", "opening_hours", "website" };
        public static string[] DetailsFieldsAtmosphere { get; set; } = { "price_level", "rating", "user_ratings_total" };

        public GoogleApiBase(string apiKey)
        {
            GoogleApiKey = apiKey;
        }

        protected static string GetPlacesQueryString(string queryType, params (string, string)[] keyValueTuples)
        {
            var queryString = GetQueryString(keyValueTuples);

            return $"{GeoPlacesUrl}{queryType}/json?{queryString}";
        }

        protected static string GetGeocodingQueryString(params (string, string)[] keyValueTuples)
        {
            var queryString = GetQueryString(keyValueTuples);

            return $"{GeocodingUrl}json?{queryString}";
        }

        protected static void ValidateSearchOptionalParams(int? radiusInMeters = 50000, string language = null, PlaceType? type = null, string keyWord = null, RankBy? rankBy = null, string moreOptionalParameters = null)
        {
            if (rankBy.HasValue && rankBy.Value == RankBy.distance)
            {
                if (radiusInMeters.HasValue)
                {
                    throw new Exception("Can't have Rankby.distance and radius at the same time'");
                }

                if (String.IsNullOrWhiteSpace(keyWord) && !type.HasValue && !(moreOptionalParameters?.Contains("name") ?? false) && !(moreOptionalParameters?.Contains("type") ?? false) && !(moreOptionalParameters?.Contains("keyword") ?? false))
                {
                    throw new Exception("When using RankBy.distance must also have either keyWorkd, name or type");
                }
            }
        }

        private static NameValueCollection GetQueryString(params (string, string)[] keyValueTuples)
        {
            NameValueCollection queryString = HttpUtility.ParseQueryString(String.Empty);

            foreach (var keyValueTuple in keyValueTuples.Where(keyValueTuple => !String.IsNullOrEmpty(keyValueTuple.Item2)))
            {
                queryString[keyValueTuple.Item1] = keyValueTuple.Item2;
            }

            queryString["key"] = GoogleApiKey;

            return queryString;
        }
    }
}