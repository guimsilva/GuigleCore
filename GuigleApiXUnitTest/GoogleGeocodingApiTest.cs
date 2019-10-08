using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GuigleApi;
using GuigleApi.Models.Address;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GuigleApiXUnitIntegrationTest
{
    public class GoogleGeocodingApiTest
    {
        private readonly GoogleGeocodingApi _googleGeocodingApi;

        private int MaxResponseContentBufferSize { get; } = 256000;
        private readonly HttpClient _client;

        private const string Address1 = "21 Park Rd, Milton QLD 4064, Australia";
        private readonly Location _addressLocation1 = new Location() {Lat = -27.4703967, Lng = 153.0042494};

        public GoogleGeocodingApiTest()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            _googleGeocodingApi = new GoogleGeocodingApi(config["GoogleApiKey"]);

            _client = new HttpClient {MaxResponseContentBufferSize = MaxResponseContentBufferSize};
        }

        [Fact]
        public async Task GetAddressFromCoordinatesAsync_ShouldReturnAddressResponse()
        {
            var address = await _googleGeocodingApi.GetAddressFromCoordinatesAsync(_client, _addressLocation1.Lat, _addressLocation1.Lng);

            Assert.Equal("OK", address.Status);
            Assert.NotEmpty(address.Results);
            Assert.NotEmpty(address.Results.Where(p => p.FormattedAddress == Address1));
        }

        [Fact]
        public async Task SearchAddressAsync_ShouldReturnAddressResponse()
        {
            var address = await _googleGeocodingApi.SearchAddressAsync(_client, Address1);

            Assert.Equal("OK", address.Status);
            Assert.NotEmpty(address.Results);
            Assert.NotEmpty(address.Results.Where(p => p.FormattedAddress == Address1));
        }
    }
}