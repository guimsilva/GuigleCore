# GuigleCore
Library to query Google Maps Api


## Usage
### Geocoding

var googleGeocodingApi = new GoogleGeocodingApi("GoogleApiKey");

var address = await googleGeocodingApi.SearchAddressAsync(httpClient, "123 Street, Suburb A");


### Places
var googlePlacesApi = new GooglePlacesApi("GoogleApiKey");

var place = await googlePlacesApi.FindPlaces(httpClient, "123 Street, Suburb A"));

## Important
Address Types and Place Types have respective enums AddressType and PlaceType, but Google often updates them, so to avoid having issues when parsing them it will also always set the types as string in another property called StringTypes.
