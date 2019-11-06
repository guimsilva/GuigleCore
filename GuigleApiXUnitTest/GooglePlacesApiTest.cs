using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GuigleApi;
using GuigleApi.Models.Address;
using GuigleApi.Models.Extension;
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
        public async Task FindBusinessWithSubtype_ShouldReturnPlaceResponse()
        {
            var place = await _googlePlacesApi.FindBusiness(_client, "Thai", _placeLocation1.Lat, _placeLocation1.Lng, 100, null, null, null, PlaceType.food);

            Assert.Equal("OK", place.Status);
            Assert.NotEmpty(place.Results);
            Assert.NotEmpty(place.Results.Where(p => p.FormattedAddress == Address1));
            Assert.NotEmpty(place.Results.Where(p => p.PlaceId == PlaceId1));
        }

        [Fact]
        public async Task FindBusinessAddressWithSubtype_ShouldReturnBusinesses()
        {
            var places = await _googlePlacesApi.FindBusinessAddress(_client, "Thai", _placeLocation1.Lat, _placeLocation1.Lng, 100, null, null, null, PlaceType.food);

            Assert.True(places.All(place => place.Addresses.Count > 0));
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

            foreach (var (location, placeId) in locationAndPlaceIds)
            {
                var place = await _googlePlacesApi.GetExactPlaceByLocation(
                    _client,
                    location.Lat,
                    location.Lng);

                Assert.NotNull(place);
                Assert.Equal(placeId, place.PlaceId);
            }
        }

        [Fact]
        public async Task ComparePlacesWithSamePlaceId_ShouldReturnTrue()
        {
            var placeResponse1 = await _googlePlacesApi.SearchPlaceByQuery(_client, "Tuk Tuk Bar", _placeLocation1.Lat, _placeLocation1.Lng);
            var place1 = placeResponse1.Results.First(p => p.PlaceId == PlaceId1);

            var place2 = await _googlePlacesApi.GetExactPlaceByAddress(_client, Address1);

            Assert.Equal(place1, place2);
        }

        [Fact]
        public async Task SearchPlaceAddressesNearBy_ShouldReturnPlaceResponse()
        {
            var places = await _googlePlacesApi.SearchPlaceAddressesNearBy(_client, _placeLocation1.Lat, _placeLocation1.Lng, null, PlaceType.food, RankBy.distance);

            Assert.NotEmpty(places);
            Assert.NotEmpty(places.SelectMany(place => place.Addresses));
        }

        [Fact]
        public async Task SearchPlaceAddressNearBy_ShouldReturnPlaceResponse()
        {
            var places = await _googlePlacesApi.SearchPlaceAddressNearBy(_client, _placeLocation1.Lat, _placeLocation1.Lng, null, PlaceType.food, RankBy.distance);

            Assert.NotEmpty(places);
            Assert.NotNull(places.First().Addresses.Single());
        }
    }
}
