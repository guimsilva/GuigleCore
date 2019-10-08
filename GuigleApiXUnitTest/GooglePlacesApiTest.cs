using System;
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

        private const string PlaceId1 = "ChIJ_5TkXqVQkWsREFoAyYbjg-g";
        private const string PlaceId2 = "ChIJ6cnyXqVQkWsRwPPRvGNJop8";
        private const string Address1 = "21 Park Rd, Milton QLD 4064, Australia";
        private readonly Location _placeLocation1 = new Location() {Lat = -27.4703967, Lng = 153.0042494};

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
            var place = await _googlePlacesApi.GetPlaceById(_client, PlaceId1);

            Assert.Equal("OK", place.Status);
            Assert.Equal(Address1, place.Result.FormattedAddress);
        }

        [Fact]
        public async Task SearchPlaceNearBy_ShouldReturnPlaceResponse()
        {
            var place = await _googlePlacesApi.SearchPlaceNearBy(_client, _placeLocation1.Lat, _placeLocation1.Lng, null, null, PlaceType.food, null, RankBy.distance);

            Assert.Equal("OK", place.Status);
            Assert.NotEmpty(place.Results);
            Assert.NotEmpty(place.Results.Where(p => p.PlaceId == PlaceId2));
        }

        [Fact]
        public async Task SearchPlaceNearByQuery_ShouldReturnPlaceResponse()
        {
            var place = await _googlePlacesApi.SearchPlaceByQuery(_client, "Tuk Tuk Bar", _placeLocation1.Lat, _placeLocation1.Lng);

            Assert.Equal("OK", place.Status);
            Assert.NotEmpty(place.Results);
            Assert.NotEmpty(place.Results.Where(p => p.PlaceId == PlaceId2));
        }
    }
}
