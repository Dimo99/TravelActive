using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TravelActive.Common.Extensions;
using TravelActive.Common.Utilities;
using TravelActive.Data;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;
namespace TravelActive.Services
{
    public abstract class DirectionsService : Service
    {
        protected IMapper mapper;
        private ApiOptions apiOptions;
        protected DirectionsService(TravelActiveContext context, IMapper mapper, IOptions<ApiOptions> apiOptions) : base(context)
        {
            this.mapper = mapper;
            this.apiOptions = apiOptions.Value;
        }

        public async Task<string> GetLatLng(string addres)
        {
            string url = apiOptions.Url;
            url += $"?address={addres}&key={apiOptions.Key}";
            using (WebClient webClient = new WebClient())
            {
                string json = await webClient.DownloadStringTaskAsync(url);
                JObject jObject = JObject.Parse(json);
                JToken results = jObject["results"].First;
                JToken geometry = results["geometry"];
                JToken location = geometry["location"];
                return $"{location["lat"]},{location["lng"]}";

            }

        }
        
        protected IEnumerable<BusStopViewModel> FindStopsInRadius(LatLng initialLocation, IEnumerable<BusStopViewModel> stops)
        {
            double initialLatRad = initialLocation.Latitude.ToRadians();
            double initialLonRad = initialLocation.Longitude.ToRadians();
            foreach (var stop in stops)
            {
                if (Math.Acos(Math.Sin(initialLatRad) * Math.Sin(stop.LatLng.Latitude.ToRadians()) + Math.Cos(initialLatRad) * Math.Cos(stop.LatLng.Latitude.ToRadians()) * Math.Cos(stop.LatLng.Longitude.ToRadians() - initialLonRad)) * 6371 <= 0.7)
                {
                    yield return stop;
                }
            }
        }
        protected StopViewModel FindNearestStop(LatLng initialLocation, IEnumerable<StopViewModel> stops)
        {
            StopViewModel minStop = stops.First();
            double minDistance = DistanceBetween(initialLocation, minStop.LatLng);
            foreach (var currentStop in stops)
            {
                double currentDistance = DistanceBetween(initialLocation, currentStop.LatLng);
                if (currentDistance < minDistance)
                {
                    minStop = currentStop;
                    minDistance = currentDistance;
                }
            }

            return minStop;
        }
        private static double DistanceBetween(double lat1, double lon1, double lat2, double lon2)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.853159616;
            return dist;
        }
        protected static double DistanceBetween(LatLng latLng1, LatLng latLng2)
        {
            return DistanceBetween(latLng1.Latitude, latLng1.Longitude, latLng2.Latitude, latLng2.Longitude);
        }
        public async Task<Directions> GetDirectionsByFoot(LatLng start, LatLng end)
        {
            string url = $"{Constants.FootRouteUrl}{start.Longitude},{start.Latitude};{end.Longitude},{end.Latitude}"; ;
            return await GetDirections(url);
        }
        protected static async Task<Directions> GetDirections(string url)
        {
            string json;
            using (WebClient webClient = new WebClient())
            {
                json = await webClient.DownloadStringTaskAsync(url);

            }

            JObject results = JObject.Parse(json);
            JToken route = results["routes"].First;
            Directions toReturn = new Directions();
            toReturn.Distance = decimal.Parse((string)route["distance"]);
            toReturn.Duration = decimal.Parse((string)route["duration"]);
            toReturn.Polylines.Add((string)route["geometry"]);
            return toReturn;
        }
        public Directions SumDirections(params Directions[] directions)
        {
            Directions sum = new Directions();
            foreach (var direction in directions)
            {
                sum += direction;
            }

            return sum;
        }
        protected List<List<BusDirections>> SortDirections(params List<BusDirections>[] directions)
        {
            var combinedDirections = new List<List<BusDirections>>();
            foreach (var item in directions)
            {
                if (item != null)
                    combinedDirections.Add(item);
            }
            //Should sort it in a better way
            return combinedDirections.OrderBy(x => x.Count).ToList();

        }
        public int Compare(Directions first, Directions second)
        {
            if (first.Duration < second.Duration)
            {
                return 1;
            }

            if (first.Duration > second.Duration)
            {
                return -1;
            }

            if (first.Distance < second.Distance)
            {
                return 1;
            }

            if (first.Distance > second.Distance)
            {
                return -1;
            }

            return 0;
        }


        //public async Task<List<Directions>> BusAlgorithm(Coordinates coordinates)
        //{
        //    // maybe instead of two queries for buses we should make just one and use select from the list cause db queries cause overhead
        //    // maybe LinkedList will be faster cause of the Remove of elements but lets leave like that for now
        //    List<BusStopViewModel> busStops = await BusStops();
        //    BusStopViewModel firstToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
        //    BusStopViewModel secondToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
        //    BusStopViewModel thirdToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
        //    busStops.AddMany(firstToLocation, secondToLocation, thirdToLocation);
        //    BusStopViewModel firstToDestination = FindNearestBusStopWithRemove(coordinates.EndPoint, busStops);
        //    BusStopViewModel secondToDestination = FindNearestBusStopWithRemove(coordinates.EndPoint, busStops);
        //    BusStopViewModel thirdToDestination = FindNearestBusStopWithRemove(coordinates.EndPoint, busStops);
        //    busStops.AddMany(firstToDestination, secondToDestination, thirdToDestination);
        //    List<Directions> directions = await GetDirectionsBuses(coordinates, firstToLocation, firstToDestination);
        //    List<Directions> directions2 = await GetDirectionsBuses(coordinates, firstToLocation, firstToDestination);
        //    List<Directions> directions3 = await GetDirectionsBuses(coordinates, firstToLocation, firstToDestination);
        //    List<Directions> directions4 = await GetDirectionsBuses(coordinates, firstToLocation, firstToDestination);
        //    List<Directions> directions5 = await GetDirectionsBuses(coordinates, firstToLocation, firstToDestination);
        //    List<Directions> directions6 = await GetDirectionsBuses(coordinates, firstToLocation, firstToDestination);
        //    List<Directions> directions7 = await GetDirectionsBuses(coordinates, firstToLocation, firstToDestination);
        //    List<Directions> directions8 = await GetDirectionsBuses(coordinates, firstToLocation, firstToDestination);
        //    List<Directions> directions9 = await GetDirectionsBuses(coordinates, firstToLocation, firstToDestination);

        //    return SortDirections(directions, directions2, directions3, directions4, directions5, directions6,
        //        directions7, directions8, directions9);
        //}



        //private async Task<List<Directions>> GetDirectionsBuses(Coordinates coordinates, BusStopViewModel busStopLocation, BusStopViewModel busStopDestination)
        //{
        //    List<Bus> busesThatWork =
        //       await GetAllBusesThatPassThroughStops(busStopLocation.StopName, busStopDestination.StopName);
        //    if (busesThatWork.Count > 0)
        //    {
        //        return await GeneratePaths(coordinates, busStopLocation, busStopDestination, busesThatWork);
        //    }

        //    return await GeneratePathsWithTransfer(coordinates, busStopLocation, busStopDestination);
        //}

        //private async Task<List<Directions>> GeneratePaths(Coordinates coordinates, BusStopViewModel busStopLocation,
        //    BusStopViewModel busStopDestination, List<Bus> busesThatWork)
        //{
        //    Directions toFirstStop = await GetDirectionsByFoot(coordinates.StartingPoint, busStopLocation.LatLng);
        //    List<Directions> busPaths = await GetBusDirections(busStopLocation, busStopDestination, busesThatWork);
        //    Directions toDestination = await GetDirectionsByFoot(busStopDestination.LatLng, coordinates.EndPoint);
        //    List<Directions> toReturn = new List<Directions>();
        //    for (int i = 0; i < busPaths.Count; i++)
        //    {
        //        toReturn.Add(SumDirections(toFirstStop, busPaths[i], toDestination));
        //    }
        //    return toReturn;
        //}

        //private async Task<List<Directions>> GetBusDirections(BusStopViewModel busStopLocation, BusStopViewModel busStopDestination, List<Bus> busesThatWork)
        //{
        //    List<Directions> toReturn = new List<Directions>();
        //    bool isBetweenStops = false;
        //    for (int i = 0; i < busesThatWork.Count; i++)
        //    {
        //        Bus currentBus = busesThatWork[i];
        //        string waypoints = "";
        //        for (int j = 0; j < currentBus.BusStops.Count; j++)
        //        {
        //            BusStop currentBusStop = currentBus.BusStops[j].BusStop;
        //            if (busStopLocation.StopName == currentBusStop.StopName)
        //            {
        //                isBetweenStops = true;
        //            }

        //            if (isBetweenStops)
        //            {
        //                if (busStopDestination.StopName == currentBusStop.StopName)
        //                {
        //                    waypoints += busStopDestination.LatLng.ToString();
        //                    break;
        //                }

        //                waypoints += currentBusStop.Latitude + "," + currentBusStop.Longitude + ";";
        //            }
        //            toReturn.Add(await GetDirectionsCar(waypoints));
        //        }
        //    }

        //    return toReturn;
        //}


        //private async Task<List<Bus>> GetAllBusesThatPassThroughStops(string firstStop, string secondStop)
        //{
        //    BusStop firstBusStop =
        //        await Context.BusStops.Include(x => x.Busses).FirstAsync(x => x.StopName == firstStop);
        //    BusStop secondBusStop =
        //        await Context.BusStops.Include(x => x.Busses).FirstAsync(x => x.StopName == secondStop);
        //    HashSet<Bus> firstBuses = new HashSet<Bus>();
        //    HashSet<Bus> secondBuses = new HashSet<Bus>();
        //    foreach (var item in firstBusStop.Busses)
        //    {
        //        firstBuses.Add(item.Bus);
        //    }
        //    foreach (var item in secondBusStop.Busses)
        //    {
        //        secondBuses.Add(item.Bus);
        //    }
        //    firstBuses.IntersectWith(secondBuses);
        //    return firstBuses.ToList();

        //}

        //private async Task<List<Directions>> GeneratePathsWithTransfer(Coordinates coordinates, BusStopViewModel busStopLocation,
        //    BusStopViewModel busStopDestination)
        //{
        //    HashSet<string> possibleBusStops = await GetPosibleBusStops(busStopLocation.StopName);
        //    HashSet<string> possibleBusStopsDestination = await GetPosibleBusStops(busStopDestination.StopName);
        //    possibleBusStops.IntersectWith(possibleBusStopsDestination);
        //    if (possibleBusStops.Count > 0)
        //    {
        //        throw new InvalidOperationException("Not possible to travel with fewer than 3 transfers !!!");
        //    }
        //    List<Directions> toReturn = new List<Directions>();
        //    foreach (var transferStop in possibleBusStops)
        //    {
        //        List<Bus> busesThatWorkToTransferStop =
        //            await GetAllBusesThatPassThroughStops(busStopLocation.StopName, transferStop);
        //        List<Bus> busesThatWorkToDestination =
        //            await GetAllBusesThatPassThroughStops(transferStop, busStopDestination.StopName);
        //        toReturn.AddRange(await GetTransferDirections(coordinates, busStopLocation, transferStop,
        //            busStopDestination, busesThatWorkToTransferStop, busesThatWorkToDestination));
        //    }
        //    return toReturn;
        //}

        //private async Task<HashSet<string>> GetPosibleBusStops(string stopName)
        //{
        //    HashSet<string> toReturn = new HashSet<string>();
        //    BusStop busStop = await Context.BusStops.FirstAsync(x=>x.StopName == stopName);
        //    foreach (var item in busStop.Busses)
        //    {
        //        var bus = item.Bus;
        //        bool isNext = false;
        //        foreach (var item2 in bus.BusStops)
        //        {
        //            var stop = item2.BusStop;
        //            if (stop.StopName == stopName)
        //            {
        //                isNext = true;
        //            }
        //            if (isNext)
        //            {
        //                toReturn.Add(stop.StopName);
        //            }
        //        }
        //    }
        //    return toReturn;
        //}

        //private async Task<List<Directions>> GetTransferDirections(Coordinates coordinates, BusStopViewModel busStopLocation, string transferStop, BusStopViewModel busStopDestination, List<Bus> busesThatWorkToTransferStop, List<Bus> busesThatWorkToDestination)
        //{
        //    List<Directions> toReturn = new List<Directions>();
        //    BusStopViewModel transferStopLocation = await GetBusStop(transferStop);
        //    Directions toTheFirstStopDirections = await GetDirectionsByFoot(coordinates.StartingPoint, busStopLocation.LatLng);
        //    List<Directions> startToTransfer =
        //        await GetBusDirections(busStopLocation, transferStopLocation, busesThatWorkToTransferStop);
        //    List<Directions> transferToEnd =
        //        await GetBusDirections(transferStopLocation, busStopLocation, busesThatWorkToDestination);
        //    Directions toTheDestination = await GetDirectionsByFoot(busStopDestination.LatLng, coordinates.EndPoint);
        //    for (int i = 0; i < startToTransfer.Count; i++)
        //    {
        //        for (var j = 0; j < transferToEnd.Count; j++)
        //        {
        //            toReturn.Add(SumDirections(toTheFirstStopDirections, startToTransfer[i], transferToEnd[j],
        //                toTheDestination));
        //        }
        //    }

        //    return toReturn;
        //}

        //private async Task<BusStopViewModel> GetBusStop(string stopName)
        //{
        //    return await Context.BusStops.ProjectTo<BusStopViewModel>().FirstAsync(x => x.StopName == stopName);
        //}

        //private async Task<List<BusStopViewModel>> BusStops()
        //{
        //    return await Context.BusStops.ProjectTo<BusStopViewModel>().ToListAsync();
        //}
    }

}