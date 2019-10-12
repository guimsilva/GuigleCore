using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GuigleApi;
using GuigleApi.Models.Address;
using GuigleApi.Models.Place;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GuigleApiXUnitIntegrationTest
{
    public class GooglePlacesApiTest
    {
        private readonly GooglePlacesApi _googlePlacesApi;

        private int MaxResponseContentBufferSize { get; } = 256000;
        private readonly HttpClient _client;

        private const string PlaceId1 = "ChIJ6cnyXqVQkWsRwPPRvGNJop8";
        private const string PlaceId2 = "ChIJvxeKP41ZkWsRM_uHzDS5mNk";
        private const string PlaceId3 = "ChIJLTwxhxBakWsRIOvwPqknAOg";
        private const string PlaceId4 = "ChIJB6j6CotZkWsRE3Lk6nffYvI";
        private const string Address1 = "21 Park Rd, Milton QLD 4064, Australia";
        private readonly Location _placeLocation1 = new Location(-27.4703967, 153.0042494);
        private readonly Location _placeLocation2 = new Location(-27.456859, 153.039852);
        private readonly Location _placeLocation3 = new Location(-27.474310, 153.029145);
        private readonly Location _placeLocation4 = new Location(-27.463087, 153.041160 );
        private readonly List<PlaceType> _politicalLocalityTypes = new List<PlaceType>() { PlaceType.locality, PlaceType.political };

        public GooglePlacesApiTest()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            _googlePlacesApi = new GooglePlacesApi(config["GoogleApiKey"]);

            _client = new HttpClient {MaxResponseContentBufferSize = MaxResponseContentBufferSize};
        }

        [Fact]
        public async Task FindBusiness_ShouldReturnPlaceResponse()
        {
            var place = await _googlePlacesApi.FindBusiness(_client, Address1, type: PlaceType.restaurant);

            Assert.Equal("OK", place.Status);
            Assert.NotEmpty(place.Results);
            Assert.NotEmpty(place.Results.Where(p => p.FormattedAddress == Address1));
        }

        [Fact]
        public async Task SearchPlaces_ShouldReturnPlaceResponse()
        {
            var place = await _googlePlacesApi.FindPlaces(_client, Address1);

            Assert.Equal("OK", place.Status);
            Assert.NotEmpty(place.Candidates);
            Assert.NotEmpty(place.Candidates.Where(p => p.FormattedAddress == Address1));
        }

        [Fact]
        public async Task GetPlaceById_ShouldReturnPlaceResponse()
        {
            var place = await _googlePlacesApi.GetPlaceDetailsById(_client, PlaceId1);

            Assert.Equal("OK", place.Status);
            Assert.Equal(Address1, place.Result.FormattedAddress);
        }

        [Fact]
        public async Task SearchPlaceNearBy_ShouldReturnPlaceResponse()
        {
            var place = await _googlePlacesApi.SearchPlaceNearBy(_client, _placeLocation1.Lat, _placeLocation1.Lng, null, null, PlaceType.food, null, RankBy.distance);

            Assert.Equal("OK", place.Status);
            Assert.NotEmpty(place.Results);
            Assert.NotEmpty(place.Results.Where(p => p.PlaceId == PlaceId1));
        }

        [Fact]
        public async Task SearchPlaceNearByQuery_ShouldReturnPlaceResponse()
        {
            var place = await _googlePlacesApi.SearchPlaceByQuery(_client, "Tuk Tuk Bar", _placeLocation1.Lat, _placeLocation1.Lng);

            Assert.Equal("OK", place.Status);
            Assert.NotEmpty(place.Results);
            Assert.NotEmpty(place.Results.Where(p => p.PlaceId == PlaceId1));
        }

        [Fact]
        public async Task SearchPlaceNearByQueryWithMoreOptionalParams_ShouldReturnPlaceResponse()
        {
            var place = await _googlePlacesApi.SearchPlaceByQuery(_client, Address1, null, null, null, "en-AU", PlaceType.establishment, ("radius", "10"));

            Assert.Equal("OK", place.Status);
            Assert.NotEmpty(place.Results);
            Assert.NotEmpty(place.Results.Where(p => p.FormattedAddress == Address1));
        }

        [Fact]
        public async Task SearchExactPlaceByAddress_ShouldReturnPlaceResponse()
        {
            var place = await _googlePlacesApi.GetExactPlaceByAddress(_client, Address1);

            Assert.NotNull(place);
            Assert.Equal(PlaceId1, place.PlaceId);
        }

        [Fact]
        public async Task SearchExactPlaceByLocation_ShouldReturnPlaceResponse()
        {
            var locationAndPlaceIds = new List<(Location, string)>
            {
                (_placeLocation2, PlaceId2),
                (_placeLocation3, PlaceId3),
                (_placeLocation4, PlaceId4)
            };

            foreach (var locationAndPlaceId in locationAndPlaceIds)
            {
                var place = await _googlePlacesApi.GetExactPlaceByLocation(
                    _client,
                    locationAndPlaceId.Item1.Lat,
                    locationAndPlaceId.Item1.Lng);

                Assert.NotNull(place);
                Assert.Equal(locationAndPlaceId.Item2, place.PlaceId);
            }
        }
    }
}
