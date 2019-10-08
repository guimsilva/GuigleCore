# GuigleCore
Library to query Google Maps Api


## Usage
### Geocoding

var googleGeocodingApi = new GoogleGeocodingApi("GoogleApiKey");

var address = await googleGeocodingApi.SearchAddressAsync(httpClient, "123 Street, Suburb A");


### Places
var googlePlacesApi = new GooglePlacesApi("GoogleApiKey");

var place = await googlePlacesApi.FindPlaces(httpClient, "123 Street, Suburb A"));
