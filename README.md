# Take Home Engineering Challenge

This is a .Net Core Console application that uses the BingMapsRESTToolkit to calculate the latitude and longitide for the user input address (optional) and postal code (required). It then calls the San Francisco Data API to get the food trucks' locations JSON response and for each of the food trucks, calculates the distance from the location input by the user using GeoCoordinates and prints out the five food trucks locations that are closest from the same location.

The console application can be run and it will prompt on the console for the user to input the address line, which is optional, and postal code, which is required, from which position the five closest food truck locations from the JSON will be provided to the user.

As the problem statement mentions, the solution can involve console/WebApi/Web frontend. The first two involve areas that I am in usually, so, I did spend time working with rendering Bing Maps and focussing it on the latitude, longitude of user interest and getting the food trucks' locations JSON in a .Net Core MVC application as well and I learned a lot in the process. In both approaches, learning about how to work with coordinates and the different algorithms used to calculate distance, etc. were new to me but the latter also involved the time required for me to work with the View to render map, etc.
