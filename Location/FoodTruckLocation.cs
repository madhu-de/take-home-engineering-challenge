using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FoodTruckLocations
{
    public class FoodTruckLocation
    {
        [JsonPropertyName("applicant")]
        public string Applicant { get; set; }

        [JsonPropertyName("facilitytype")]
        public string FacilityType { get; set; }

        [JsonPropertyName("locationdescription")]
        public string LocationDescription { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("block")]
        public string Block { get; set; }

        [JsonPropertyName("lot")]
        public string Lot { get; set; }

        [JsonPropertyName("fooditems")]
        public string FoodItems { get; set; }

        [JsonPropertyName("latitude")]
        public string Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public string Longitude { get; set; }

        [JsonPropertyName("schedule")]
        public string Schedule { get; set; }

        public double DistanceFromCurrentLocation { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("coordinates")]
        public List<double> Coordinates { get; private set; }

        public Location()
        {
            Coordinates = new List<double>();
        }
    }
}
