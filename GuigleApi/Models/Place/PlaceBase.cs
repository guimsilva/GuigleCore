using System.Collections.Generic;
using GuigleApi.Models.Address;
using Newtonsoft.Json;

namespace GuigleApi.Models.Place
{
    public class PlaceBase
    {
        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("opening_hours")]
        public OpeningHours OpeningHours { get; set; }

        [JsonProperty("photos")]
        public List<Photo> Photos { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// key is place_id and value is scope
        /// </summary>
        [JsonProperty("alt_ids")]
        public Dictionary<string, string> AltIds { get; set; }

        /// <summary>
        /// Scale is from 0 to 4
        /// </summary>
        [JsonProperty("price_level")]
        public int PriceLevel { get; set; }

        /// <summary>
        /// From 1.0 to 5.0
        /// </summary>
        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("user_ratings_total")]
        public double UserRatingsTotal { get; set; }

        /// <summary>
        /// Token used to request Place Details
        /// </summary>
        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("vicinity")]
        public string Vicinity { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("permanently_closed")]
        public bool PermanentlyClosed { get; set; }
    }
}
