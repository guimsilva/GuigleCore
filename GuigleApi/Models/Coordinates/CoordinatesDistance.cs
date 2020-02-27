using System;
using GuigleApi.Models.Address;
using GuigleApi.Models.Coordinates;

namespace GuigleApi.Models.Coordinates
{
    public static class CoordinatesDistance
    {
        public static double DistanceTo(this Coordinates baseCoordinates, Coordinates targetCoordinates)
        {
            return DistanceTo(baseCoordinates, targetCoordinates, UnitOfLength.Kilometers);
        }

        public static double DistanceTo(this Coordinates baseCoordinates, Coordinates targetCoordinates, UnitOfLength unitOfLength)
        {
            var baseRad = Math.PI * baseCoordinates.Latitude / 180;
            var targetRad = Math.PI * targetCoordinates.Latitude / 180;
            var theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
            var thetaRad = Math.PI * theta / 180;

            double dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);
            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return unitOfLength.ConvertFromMiles(dist);
        }

        public static double EarhRadiusInKm { get; } = 6371;

        /// <summary>
        /// Gets the rounded distance in kilometers
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng1"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static int GetDistance(double lat1, double lat2, double lng1, double lng2)
        {
            return Convert.ToInt32(Math.Round(GetPreciseDistance(lat1, lat2, lng1, lng2)));
        }

        /// <summary>
        /// Gets the precise distance in kilometers
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng1"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double GetPreciseDistance(double lat1, double lat2, double lng1, double lng2)
        {
            var distance = new Coordinates(lat1, lng1)
                .DistanceTo(
                    new Coordinates(lat2, lng2),
                    UnitOfLength.Kilometers
                );

            return distance;
        }

        public static Location Travel(Location startLocation, double initialDegree, double distanceInKm)
        {
            double ib = DegreesToRadians(initialDegree);
            double startLat1Rad = DegreesToRadians(startLocation.Lat);
            double startLon1Rad = DegreesToRadians(startLocation.Lng);
            double distRad = distanceInKm / EarhRadiusInKm ;

            double a = Math.Sin(distRad) * Math.Cos(startLat1Rad);
            double lat2 = Math.Asin(Math.Sin(startLat1Rad) * Math.Cos(distRad) + a * Math.Cos(ib));
            double lon2 = startLon1Rad + Math.Atan2(Math.Sin(ib) * a, Math.Cos(distRad) - Math.Sin(startLat1Rad) * Math.Sin(lat2));

            return new Location(RadiansToDegrees(lat2), RadiansToDegrees(lon2));
        }

        public static double DegreesToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }
    }
}
