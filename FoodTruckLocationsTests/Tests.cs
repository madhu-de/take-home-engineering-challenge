using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public async Task TestCurrentCoordinatesWithSFOAirportPostalCode()
        {
            var currentAddressLine = string.Empty;
            var currentPostalCode = "94128";

            var currentCoordinates = await FoodTruckLocations.Program.GetCurrentCoordinates(currentAddressLine, currentPostalCode);

            Assert.IsNotNull(currentCoordinates);

            Assert.AreEqual(currentCoordinates.Type, "Point");
            Assert.AreEqual(currentCoordinates.Coordinates.Count, 2);

            var difference = Math.Abs(37.628631591796875 * .00001);
            Assert.IsTrue(Math.Abs(37.628631591796875 - currentCoordinates.Coordinates[0]) <= difference);

            difference = Math.Abs(-122.385986328125 * .00001);
            Assert.IsTrue(Math.Abs(-122.385986328125 - currentCoordinates.Coordinates[1]) <= difference);
        }

        [Test]
        public async Task TestCurrentCoordinatesWithMSSFOAddress()
        {
            var currentAddressLine = "865 Market St Westfield";
            var currentPostalCode = "94103";

            var currentCoordinates = await FoodTruckLocations.Program.GetCurrentCoordinates(currentAddressLine, currentPostalCode);
            Assert.IsNotNull(currentCoordinates);

            Assert.AreEqual(currentCoordinates.Type, "Point");
            Assert.AreEqual(currentCoordinates.Coordinates.Count, 2);

            var difference = Math.Abs(37.784099 * .00001);
            Assert.IsTrue(Math.Abs(37.784099 - currentCoordinates.Coordinates[0]) <= difference);

            difference = Math.Abs(-122.406434 * .00001);
            Assert.IsTrue(Math.Abs(-122.406434 - currentCoordinates.Coordinates[1]) <= difference);
        }

        [Test]
        public async Task TestGetFoodTruckLocations()
        {
            var foodTruckLocations = await FoodTruckLocations.Program.GetFoodTruckLocations();

            Assert.IsNotNull(foodTruckLocations);
            Assert.AreEqual(foodTruckLocations.Count, 644);

            var expectedFTL = foodTruckLocations.Find(ft => ft.Applicant == "Rita's Catering");
            Assert.IsNotNull(expectedFTL);

            Assert.AreEqual(expectedFTL.Latitude, "37.7806943774082");
            Assert.AreEqual(expectedFTL.Longitude, "-122.409668813219");
        }

        [Test]
        public async Task TestCalculateDistancesToFoodTrucks()
        {
            var currentAddressLine = "865 Market St Westfield";
            var currentPostalCode = "94103";

            var currentCoordinates = await FoodTruckLocations.Program.GetCurrentCoordinates(currentAddressLine, currentPostalCode);
            var foodTruckLocations = await FoodTruckLocations.Program.GetFoodTruckLocations();

            var foodTruckLocationsWithDistance = FoodTruckLocations.Program.CalculateDistancesToFoodTrucks(currentCoordinates, foodTruckLocations);

            Assert.IsNotNull(foodTruckLocationsWithDistance);
            Assert.AreEqual(foodTruckLocationsWithDistance.Count, 644);

            var expectedFTL = foodTruckLocations.Find(ft => ft.Applicant == "Rita's Catering");
            Assert.IsNotNull(expectedFTL);

            Assert.AreEqual(expectedFTL.Latitude, "37.7806943774082");
            Assert.AreEqual(expectedFTL.Longitude, "-122.409668813219");

            var difference = Math.Abs(expectedFTL.DistanceFromCurrentLocation * .00001);
            Assert.IsTrue(Math.Abs(expectedFTL.DistanceFromCurrentLocation - 473.83962683427654) <= difference);
        }
    }
}