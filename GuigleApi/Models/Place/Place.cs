using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GuigleApi.Models.Address;
using GuigleApi.Models.Response;
using Newtonsoft.Json;

namespace GuigleApi.Models.Place
{
    public class Place : IPlace, IEquatable<IPlace>
    {
        public string PlaceId { get; set; }
        public string Icon { get; set; }
        public Geometry Geometry { get; set; }
        public string Name { get; set; }
        public OpeningHours OpeningHours { get; set; }
        public List<Photo> Photos { get; set; }
        public string Scope { get; set; }
        public Dictionary<string, string> AltIds { get; set; }
        public int PriceLevel { get; set; }
        public double Rating { get; set; }
        public double UserRatingsTotal { get; set; }
        public string Reference { get; set; }
        public string Vicinity { get; set; }
        public string FormattedAddress { get; set; }
        public bool PermanentlyClosed { get; set; }
        public List<PlaceType> Types { get; set; }
        public List<string> StringTypes { get; set; }
        public List<Address.Address> Addresses { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Place);
        }

        public bool Equals(IPlace other)
        {
            if (other == null)
            {
                return false;
            }

            return this.PlaceId == other.PlaceId;
        }

        public static async Task<Response<Place>> ParseResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw await Response<PlaceT>.ResponseError(response);
            }

            var content = await response.Content.ReadAsStringAsync();

            Response<Place> result;
            try
            {
                result = ParseResponse(JsonConvert.DeserializeObject<Response<PlaceT>>(content));
                result.Results?.ForEach(SetPlaceStringTypes);
                result.Candidates?.ForEach(SetPlaceStringTypes);
            }
            catch (JsonSerializationException e)
            {
                result = ParseResponse(JsonConvert.DeserializeObject<Response<PlaceS>>(content));
            }

            if (result is null)
            {
                throw await Response<PlaceT>.ResponseError(response);
            }

            if (result.Status == "INVALID_REQUEST")
            {
                throw new HttpRequestException(result.ErrorMessage ?? "INVALID_REQUEST");
            }

            return result;
        }

        public static void SetPlaceStringTypes(IPlace place)
        {
            if (place.Types is null || (place.StringTypes?.Any() ?? false)) return;
            place.StringTypes = new List<string>();
            place.Types.ForEach(type => place.StringTypes.Add(type.ToString()));
        }

        private static Response<Place> ParseResponse(Response<PlaceT> placeT)
        {
            if (placeT is null)
            {
                return null;
            }

            return new Response<Place>()
            {
                Candidates = placeT.Candidates?.Select(ParsePlace).ToList(),
                ErrorMessage = placeT.ErrorMessage,
                HtmlAttributions = placeT.HtmlAttributions,
                NextPageToken = placeT.NextPageToken,
                Result = ParsePlace(placeT.Result),
                Results = placeT.Results?.Select(ParsePlace).ToList(),
                Status = placeT.Status
            };
        }

        private static Response<Place> ParseResponse(Response<PlaceS> placeS)
        {
            if (placeS is null)
            {
                return null;
            }

            return new Response<Place>()
            {
                Candidates = placeS.Candidates?.Select(ParsePlace).ToList(),
                ErrorMessage = placeS.ErrorMessage,
                HtmlAttributions = placeS.HtmlAttributions,
                NextPageToken = placeS.NextPageToken,
                Result = ParsePlace(placeS.Result),
                Results = placeS.Results?.Select(ParsePlace).ToList(),
                Status = placeS.Status
            };
        }

        public static Place ParsePlace(IPlace place)
        {
            if (place is null)
            {
                return null;
            }

            return new Place()
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
        }
    }
}
