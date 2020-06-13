using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using BingMapsRESTToolkit;
using GeoCoordinatePortable;

[assembly: InternalsVisibleTo("FoodTruckLocationsTests")]
namespace FoodTruckLocations
{
    class Program
    {
        private const string BingMapsKey = "Ap9BwF3pMipJo19wG6au12NRI4HtkuFUgib9MktVxFkolOwZlGbp2kEpdxfuJ_FV";
        private const string FoodTruckJsonUrl = "https://data.sfgov.org/resource/rqzj-sfat.json";

        private static readonly HttpClient client = new HttpClient();

        static async Task Main()
        {
            var currentAddressLine = GetAddress();
            var currentPostalCode = GetPostalCode();

            if (CheckCurrentAddressInput(ref currentAddressLine, ref currentPostalCode))
            {
                //Get coordinate for the address/postal code input from the user
                var currentCoordinates = await GetCurrentCoordinates(currentAddressLine, currentPostalCode);

                currentCoordinates = await CheckCurrentCoordinates(currentCoordinates);

                if (currentCoordinates.Coordinates == null || currentCoordinates.Coordinates.Count != 2)
                {
                    Console.WriteLine("The coordinates for the current input location could not be retrieved; the program will now exit.");
                    return;
                }

                //Call SF Gov URL to get back published Food Truck locations JSON
                var foodTruckLocations = await GetFoodTruckLocations();

                //Calculate distance of food trucks from the current coordinate
                var foodTruckLocationsWithDistances = CalculateDistancesToFoodTrucks(currentCoordinates, foodTruckLocations);

                if (foodTruckLocationsWithDistances == null || foodTruckLocations.Count == 0)
                {
                    Console.WriteLine("The distance from the current cooordinate to the listed food trucks could not be calculated; the program will now exit.");
                    return;
                }

                Console.WriteLine("The five closest food truck locations are: ");
                int count = 0;
                foreach (var f in foodTruckLocationsWithDistances)
                {
                    //and print 5 closest food trucks
                    Console.WriteLine(f.Applicant + " " + f.Address + " " + f.FoodItems + " Distance to: " + f.DistanceFromCurrentLocation + " meters");
                    count++;

                    if (count == 5)
                        break;
                }
            }
            else
            {
                Console.WriteLine("The postal code input has not been provided; the program will now exit.");
                return;
            }
        }

        private static string GetAddress()
        {
            Console.WriteLine("Please enter the current address line (optional).");
            var currentAddressLine = Console.ReadLine();

            return currentAddressLine;
        }

        private static string GetPostalCode()
        {
            Console.WriteLine("Please enter the current postal code (required).");
            var currentPostalCode = Console.ReadLine();

            return currentPostalCode;
        }

        private static bool CheckCurrentAddressInput(ref string currentAddressLine, ref string currentPostalCode)
        {
            if (string.IsNullOrEmpty(currentPostalCode))
            {
                Console.WriteLine("The postal code input is required to calculate the current coordinate and provide the closest food truck locations without which the program cannot continue.");
                currentAddressLine = GetAddress();
                currentPostalCode = GetPostalCode();

                if (string.IsNullOrEmpty(currentPostalCode))
                {
                    return false;
                }
            }

            return true;
        }

        internal static async Task<Location> GetCurrentCoordinates(string currentAddressLine, string currentPostalCode )
        {
            var currentLocationCoordinates = new Location { Type = "Point" };
            var currentAddress = new SimpleAddress { AddressLine = currentAddressLine, PostalCode = currentPostalCode };

            var geocodeRequest = new GeocodeRequest()
            {
                Address = currentAddress,
                IncludeIso2 = true,
                IncludeNeighborhood = true,
                MaxResults = 25,
                BingMapsKey = BingMapsKey
            };
            var response = await geocodeRequest.Execute();

            if (response != null &&
                response.ResourceSets != null &&
                response.ResourceSets.Length > 0 &&
                response.ResourceSets[0].Resources != null &&
                response.ResourceSets[0].Resources.Length > 0)
            {
                var result = response.ResourceSets[0].Resources[0] as BingMapsRESTToolkit.Location;

                var coords = result.Point.Coordinates;
                if (coords != null && coords.Length == 2)
                {
                    var lat = coords[0];
                    var lng = coords[1];

                    Console.WriteLine($"Geocode Results - Lat: {lat} / Long: {lng}");
                    currentLocationCoordinates.Coordinates.Add(lat);
                    currentLocationCoordinates.Coordinates.Add(lng);
                }
            }

            return currentLocationCoordinates;
        }

        private static async Task<Location> CheckCurrentCoordinates(Location currentCoordinates)
        {
            if (currentCoordinates.Coordinates == null || currentCoordinates.Coordinates.Count != 2)
            {
                Console.WriteLine("The coordinates for the current input location could not be retrieved. Please try again.");
                string currentAddressLine = GetAddress();
                string currentPostalCode = GetPostalCode();
                CheckCurrentAddressInput(ref currentAddressLine, ref currentPostalCode);
                currentCoordinates = await GetCurrentCoordinates(currentAddressLine, currentPostalCode);
            }

            return currentCoordinates;
        }

        internal static async Task<List<FoodTruckLocation>> GetFoodTruckLocations()
        {
            List<FoodTruckLocation> foodTruckLocations = new List<FoodTruckLocation>();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var streamTask = client.GetStreamAsync(FoodTruckJsonUrl);
           
            try
            {
                foodTruckLocations = await JsonSerializer.DeserializeAsync<List<FoodTruckLocation>>(await streamTask);
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            return foodTruckLocations;
        }

        internal static List<FoodTruckLocation> CalculateDistancesToFoodTrucks(
            Location currentLocationCoordinates, List<FoodTruckLocation> foodTruckLocations)
        {
            foreach (var foodTruckLocation in foodTruckLocations)
            {
                try
                {
                   var sCoord = new GeoCoordinate(currentLocationCoordinates.Coordinates[0], currentLocationCoordinates.Coordinates[1]);
                        var eCoord = new GeoCoordinate(double.Parse(foodTruckLocation.Latitude), double.Parse(foodTruckLocation.Longitude));

                   foodTruckLocation.DistanceFromCurrentLocation = sCoord.GetDistanceTo(eCoord);
                }
                catch(Exception exc) 
                {
                    Console.WriteLine($"Exception was encountered in calculating distance for food truck: {foodTruckLocation.Applicant} "
                        + exc.Message);
                    continue;
                }
            }

            foodTruckLocations?.Sort(delegate (FoodTruckLocation one, FoodTruckLocation two)
            {
                return one.DistanceFromCurrentLocation.CompareTo(two.DistanceFromCurrentLocation);
            });

            return foodTruckLocations;
        }
    }
}
