using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GuigleApi.Models.Address
{
    [JsonObject]
    public class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }

        public Location(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }
    }
}
