# GuigleCore
Library to query Google Maps Api


## Usage
### Geocoding

var googleGeocodingApi = new GoogleGeocodingApi("GoogleApiKey");

var address = await googleGeocodingApi.SearchAddressAsync(httpClient, "123 Street, Suburb A");


### Places
var googlePlacesApi = new GooglePlacesApi("GoogleApiKey");

var places = await googlePlacesApi.FindPlaces(httpClient, "123 Street, Suburb A"));

OR

var place = await googlePlacesApi.GetExactPlaceByAddress(_client, "123 Street, Suburb A");

OR

var place = await googlePlacesApi.GetExactPlaceByLocation(_client, -11.111, 11.111);

## Important
Address Types and Place Types have respective enums AddressType and PlaceType, but Google often updates them, so to avoid having issues when parsing them it will also always set the types as string in another property called StringTypes.
