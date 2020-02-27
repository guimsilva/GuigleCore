using GuigleApi.Models.Address;
using GuigleApi.Models.Coordinates;
using Xunit;

namespace GuigleApiXUnitIntegrationTest
{
    public class CoordinatesTest
    {
        private readonly Location _placeLocation1 = new Location(-27.4703967, 153.0042494);

        [Fact]
        public void GetDistance_Travel_North_Should_Match()
        {
            var distanceInKm = 1;
            var directionDegree = 0; // North
            
            var travelLocation = CoordinatesDistance.Travel(_placeLocation1, directionDegree, distanceInKm);

            var distanceResult = CoordinatesDistance.GetDistance(_placeLocation1.Lat, travelLocation.Lat, _placeLocation1.Lng, travelLocation.Lng);

            Assert.Equal(distanceInKm, distanceResult);
        }

        [Fact]
        public void GetDistance_Travel_South_Should_Match()
        {
            var distanceInKm = 1;
            var directionDegree = 180; // South

            var travelLocation = CoordinatesDistance.Travel(_placeLocation1, directionDegree, distanceInKm);

            var distanceResult = CoordinatesDistance.GetDistance(_placeLocation1.Lat, travelLocation.Lat, _placeLocation1.Lng, travelLocation.Lng);

            Assert.Equal(distanceInKm, distanceResult);
        }
    }
}
