using System;
using GuigleApi.Models.Place;
using Newtonsoft.Json;

namespace GuigleApi.Models.Address
{
    [JsonObject]
    public class Location : IEquatable<Location>
    {
        [JsonIgnore]
        private double _comparisonTolerance = 0.00001;

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }

        public Location(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Location);
        }

        public bool Equals(Location other)
        {
            if (other == null)
            {
                return false;
            }

            return Math.Abs(this.Lat - other.Lat) < _comparisonTolerance
                   && Math.Abs(this.Lng - other.Lng) < _comparisonTolerance;
        }
    }
}
