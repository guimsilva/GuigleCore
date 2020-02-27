using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GuigleApi;
using GuigleApi.Models.Address;
using GuigleApi.Models.Extension;
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
        private readonly Location _addressLocation1 = new Location(-27.4703967, 153.0042494);

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
        public async Task GetAddressFromCoordinatesShouldReturnAddressResponse()
        {
            var address = await _googleGeocodingApi.GetAddressFromCoordinates(_client, _addressLocation1.Lat, _addressLocation1.Lng).ConfigureAwait(false);

            Assert.Equal("OK", address.Status);
            Assert.NotEmpty(address.Results);
            Assert.NotEmpty(address.Results.Where(p => p.FormattedAddress == Address1));
        }

        [Fact]
        public async Task SearchAddressShouldReturnAddressResponse()
        {
            var address = await _googleGeocodingApi.SearchAddress(_client, Address1).ConfigureAwait(false);

            Assert.Equal("OK", address.Status);
            Assert.NotEmpty(address.Results);
            Assert.NotEmpty(address.Results.Where(p => p.FormattedAddress == Address1));
        }

        [Fact]
        public async Task GetAddressPartsFromSearchAddressResponseShouldReturnAddressResponse()
        {
            var address = await _googleGeocodingApi.SearchAddress(_client, Address1).ConfigureAwait(false);

            Assert.Equal("OK", address.Status);
            Assert.Equal("Milton", address.Results?.GetSuburb().SuburbLongName);
            Assert.Equal("Brisbane", address.Results?.GetCity().CityShortName);
            Assert.Equal("QLD", address.Results?.GetState()?.StateShortName);
            Assert.Equal("Australia", address.Results?.GetCountry()?.CountryLongName);
        }

        [Fact]
        public async Task GetAddressPartsFromSearchAddressResultShouldReturnAddressResponse()
        {
            var addressResponse = await _googleGeocodingApi.SearchAddress(_client, Address1).ConfigureAwait(false);
            var address = addressResponse.Results.First();

            Assert.Equal("Milton", address.SuburbShortName);
            Assert.Equal("Brisbane", address.CityShortName);
            Assert.Equal("QLD", address.StateShortName);
            Assert.Equal("Australia", address.CountryLongName);
        }
    }
}
