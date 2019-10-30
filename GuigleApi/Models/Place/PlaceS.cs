using Newtonsoft.Json;
using System.Collections.Generic;

namespace GuigleApi.Models.Place
{
    [JsonObject]
    public class PlaceS : PlaceBase, IPlace
    {
        [JsonIgnore]
        public List<PlaceType> Types { get; set; }

        [JsonProperty("types")]
        public List<string> StringTypes { get; set; }

        public static PlaceS Parse(IPlace place)
        {
            if (place is null)
            {
                return null;
            }

            var placeS = new PlaceS()
            {
                AltIds = place.AltIds,
                FormattedAddress = place.FormattedAddress,
                Geometry = place.Geometry,
                Icon = place.Icon,
                Name = place.Name,
                OpeningHours = place.OpeningHours,
                PermanentlyClosed = place.PermanentlyClosed,
                Photos = place.Photos,
                PlaceId = place.PlaceId,
                PriceLevel = place.PriceLevel,
                Rating = place.Rating,
                Reference = place.Reference,
                Scope = place.Scope,
                StringTypes = place.StringTypes,
                Types = place.Types,
                UserRatingsTotal = place.UserRatingsTotal,
                Vicinity = place.Vicinity
            };

            Place.SetPlaceStringTypes(placeS);
            return placeS;
        }
    }
}
