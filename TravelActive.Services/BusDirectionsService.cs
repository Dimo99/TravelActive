using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelActive.Common.Extensions;
using TravelActive.Common.Utilities;
using TravelActive.Data;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;

namespace TravelActive.Services
{
    public class BusDirectionsService : DirectionsService
    {
        public BusDirectionsService(TravelActiveContext context, IMapper mapper, IOptions<ApiOptions> options)
            : base(context, mapper, options)
        {
        }
        private BusStopViewModel FindNearestBusStopWithRemove(LatLng coordinatesStartingPoint, List<BusStopViewModel> busStops)
        {
            BusStopViewModel nearestStop = FindNearestStop(coordinatesStartingPoint, busStops) as BusStopViewModel;
            busStops.Remove(nearestStop);
            return nearestStop;
        }

        private async Task<Directions> GetDirectionsCar(string waypoints)
            => await GetDirections($"{Constants.CarRouteUrl}{waypoints}?overview=full");

        private async Task<List<BusStopViewModel>> BusStops(int cityId)
        {
            return await Context.BusStops.Where(x => x.CityId == cityId).ProjectTo<BusStopViewModel>().ToListAsync();
        }
        public async Task<List<List<BusDirections>>> BusAlgorithm(Coordinates coordinates, string startPlace, string endPlace, int cityId)
        {
            StaticData.ListStopAccessibilities = Context.StopsAccessibility.Where(x => x.InitialStop.CityId == cityId).ToList();
            StaticData.ListStopOrdered = Context.StopsOrdered.Where(x => x.BusStop.CityId == cityId).Include(x => x.BusStop).ToList();
            List<BusStopViewModel> toLocation = new List<BusStopViewModel>();
            List<BusStopViewModel> busStops = await BusStops(cityId);
            toLocation = FindStopsInRadius(coordinates.StartingPoint, busStops).ToList();
            int stopsTaken = 3 - toLocation.Count;
            for (int i = 0; i < stopsTaken; i++)
            {
                toLocation.Add(FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops));
            }
            if (stopsTaken > 0)
                busStops.AddRange(toLocation.Skip(3 - stopsTaken));
            List<BusStopViewModel> toDestination = new List<BusStopViewModel>();
            toDestination = FindStopsInRadius(coordinates.EndPoint, busStops).ToList();
            stopsTaken = 3 - toDestination.Count;
            for (int i = 0; i < stopsTaken; i++)
            {
                toDestination.Add(FindNearestBusStopWithRemove(coordinates.EndPoint, busStops));
            }
            busStops.AddRange(toDestination.Skip(3 - stopsTaken));
            bool tryOnFoot = false;
            var paths = new List<List<StopAccessibleViewModel>>();
            List<List<StopAccessibleViewModel>> pathsWithTransfer = new List<List<StopAccessibleViewModel>>();
            List<List<StopAccessibleViewModel>> directPaths = new List<List<StopAccessibleViewModel>>();
            for (int i = 0; i < toLocation.Count; i++)
            {
                for (int j = 0; j < toDestination.Count; j++)
                {
                    if (toLocation[i].Id == toDestination[j].Id)
                    {
                        tryOnFoot = true;
                        continue;
                    }

                    var result = GetBiDirectionalBFSAsync(toLocation[i].Id, toDestination[j].Id, coordinates.EndPoint);
                    if (result == null)
                    {
                        continue;
                    }
                    if (result.Count == 2)
                    {
                        pathsWithTransfer.Add(result[0]);
                        directPaths.Add(result[1]);
                    }
                    else
                    {
                        if (result[0].Count == 1)
                        {
                            directPaths.Add(result[0]);
                        }
                        else
                        {
                            pathsWithTransfer.Add(result[0]);
                        }
                    }
                }
            }
            if (directPaths.Count != 0)
                paths.Add(BestDirectPath(coordinates.StartingPoint, coordinates.EndPoint, directPaths));
            if (pathsWithTransfer.Count != 0)
                paths.Add(BestDirectPath(coordinates.StartingPoint, coordinates.EndPoint, pathsWithTransfer));
            List<List<BusDirections>> busDirections = new List<List<BusDirections>>();
            for (int i = 0; i < paths.Count; i++)
            {
                busDirections.Add(await GetDirectionsBus(coordinates, paths[i], startPlace, endPlace));
            }
            if (DistanceBetween(coordinates.StartingPoint, coordinates.EndPoint) < 0.5 || tryOnFoot)
            {
                Directions d = await GetDirectionsByFoot(coordinates.StartingPoint, coordinates.EndPoint);
                BusDirections footDirections = new BusDirections();
                footDirections.Polyline = d.Polylines.First();
                footDirections.Distance = d.Distance;
                footDirections.Duration = d.Duration;
                footDirections.LocationEnd = startPlace;
                footDirections.LocationStart = endPlace;
                footDirections.Method = "foot";
                busDirections.Insert(0, new List<BusDirections>() { footDirections });
            }
            return busDirections.ToList();
        }

        private List<StopAccessibleViewModel> BestDirectPath(LatLng startPoint, LatLng endPoint, List<List<StopAccessibleViewModel>> directPaths)
        {
            List<double> timeToInitialStop = new List<double>();
            List<double> timeToDestinationStop = new List<double>();

            foreach (var path in directPaths)
            {
                BusStopViewModel busStopInitial =
                    Mapper.Map<BusStopViewModel>(Context.BusStops.First(x => x.Id == path[0].InitialStopId));
                BusStopViewModel busStopDestination =
                    Mapper.Map<BusStopViewModel>(Context.BusStops.First(x => x.Id == path[path.Count - 1].DestStopId));
                timeToInitialStop.Add(DistanceBetween(busStopInitial.LatLng, startPoint));
                timeToDestinationStop.Add(DistanceBetween(busStopDestination.LatLng, endPoint));
            }

            double min = timeToInitialStop[0] + timeToDestinationStop[0];
            int minIndex = 0;
            for (var i = 1; i < timeToInitialStop.Count; i++)
            {
                if (min > timeToInitialStop[i] + timeToDestinationStop[i])
                {
                    min = timeToInitialStop[i] + timeToDestinationStop[i];
                    minIndex = i;
                }
            }

            return directPaths[minIndex];
        }

        private int CalcTime(double distanceBetween)
        {
            return (int)(distanceBetween / 0.07);
        }


        private async Task<List<BusDirections>> GetDirectionsBus(Coordinates coordinates, List<StopAccessibleViewModel> path, string startPlace, string endPlace)
        {
            List<BusDirections> busDirections = new List<BusDirections>();

            var firstStop = path[0];
            var firstStopName = GetBusStop(firstStop.InitialStopId).StopName;
            Directions directionsToTheFirstStop = await GetDirectionsByFoot(coordinates.StartingPoint,
                Context.BusStops.ProjectTo<BusStopViewModel>().First(x => x.Id == firstStop.InitialStopId).LatLng);
            var byFoot = new BusDirections()
            {
                Method = "foot",
                Polyline = directionsToTheFirstStop.Polylines.First(),
                Distance = directionsToTheFirstStop.Distance,
                Duration = directionsToTheFirstStop.Duration,
                LocationStart = startPlace,
                LocationEnd = firstStopName
            };
            busDirections.Add(byFoot);
            foreach (var item in path)
            {
                string waypoints = GetWayPoints(item);
                string f = GetBusStop(item.InitialStopId).StopName;
                string second = GetBusStop(item.DestStopId).StopName;
                Directions directionsBetweenStops = await GetDirectionsCar(waypoints);
                string polyline = "";
                foreach (var s in directionsBetweenStops.Polylines)
                {
                    polyline = polyline.PolylineAdd(s);
                }
                busDirections.Add(new BusDirections()
                {
                    Bus = GetBusName(item.BusId),
                    Method = "bus",
                    Distance = directionsBetweenStops.Distance,
                    Duration = directionsBetweenStops.Duration,
                    Polyline = polyline,
                    LocationStart = f,
                    LocationEnd = second
                });
            }
            var lastStop = path[path.Count - 1];
            var lastStopName = GetBusStop(lastStop.DestStopId).StopName;
            Directions directionsToDestination = await GetDirectionsByFoot(Context.BusStops
                .ProjectTo<BusStopViewModel>().First(x => x.Id == lastStop.DestStopId).LatLng, coordinates.EndPoint);
            busDirections.Add(new BusDirections()
            {
                Polyline = directionsToDestination.Polylines.First(),
                Method = "foot",
                Distance = directionsToDestination.Distance,
                Duration = directionsToDestination.Duration,
                LocationStart = lastStopName,
                LocationEnd = endPlace
            });

            return busDirections;
        }

        private BusStop GetBusStop(int firstStopInitialStopId)
        {
            return Context.BusStops.Find(firstStopInitialStopId);
        }

        private string GetBusName(int? itemBusId)
        {
            return Context.Busses.Find(itemBusId).BusName;

        }

        private string GetWayPoints(StopAccessibleViewModel item)
        {
            string waypoints = "";
            List<BusStop> busStops = StaticData.ListStopOrdered.Where(x => x.BusId == item.BusId).OrderBy(x => x.Id).Select(x => x.BusStop).ToList();
            bool flag = false;
            foreach (var busStop in busStops)
            {
                if (busStop.Id == item.InitialStopId)
                {
                    flag = true;
                }

                if (busStop.Id == item.DestStopId && flag)
                {

                    waypoints += $"{busStop.Longitude},{busStop.Latitude}";
                    flag = false;
                    break;
                }
                if (flag)
                {
                    waypoints += $"{busStop.Longitude},{busStop.Latitude};";
                }
            }

            return waypoints;
        }
        //TODO : Fix the method
        // First learn some graph theory then apply it here
        private List<List<StopAccessibleViewModel>> GetBiDirectionalBFSAsync(int initialId, int destId, LatLng destination)
        {
            var path = mapper.Map<List<StopAccessibleViewModel>>(StaticData.ListStopAccessibilities.
                Where(s => s.InitialStopId == initialId && s.DestStopId == destId).ToList());

            var toReturn = new List<List<StopAccessibleViewModel>>();
            if (path.Count != 0)
            {
                return new List<List<StopAccessibleViewModel>>()
                {
                    new List<StopAccessibleViewModel>()
                    {
                        Max(path)
                    }
                };
            }
            HashSet<int> usedInitial = new HashSet<int>();
            HashSet<int> usedDestination = new HashSet<int>();
            Queue<int> initialStops = new Queue<int>();
            Queue<int> destinationStops = new Queue<int>();
            initialStops.Enqueue(initialId);
            destinationStops.Enqueue(destId);
            Dictionary<int, int> forwardPrev = new Dictionary<int, int>();
            Dictionary<int, int> backwardPrev = new Dictionary<int, int>();
            HashSet<int> forwardMoved = new HashSet<int>();
            HashSet<int> backwardMoved = new HashSet<int>();
            List<StopAccessibleViewModel> pathForContinueOnFoot = null;
            int counter = 0;
            while (initialStops.Count != 0 || destinationStops.Count != 0)
            {
                if (counter == 1)
                {
                    break;
                }
                var currentInitialId = initialStops.Dequeue();
                var currentDestId = destinationStops.Dequeue();
                var possibleGoingId = StaticData.ListStopAccessibilities
                    .Where(s => s.InitialStopId == currentInitialId && !usedInitial.Contains(s.DestStopId))
                    .Select(x => x.DestStopId)
                    .ToHashSet();
                var possibleCommingId = StaticData.ListStopAccessibilities
                    .Where(s => s.DestStopId == currentDestId && !usedDestination.Contains(s.InitialStopId))
                    .Select(x => x.InitialStopId)
                    .ToHashSet();
                foreach (var item in possibleGoingId)
                {
                    initialStops.Enqueue(item);
                    usedInitial.Add(item);
                    forwardMoved.Add(item);
                    forwardPrev[item] = currentInitialId;
                }
                foreach (var item in possibleCommingId)
                {
                    destinationStops.Enqueue(item);
                    usedDestination.Add(item);
                    backwardMoved.Add(item);
                    backwardPrev[item] = currentDestId;
                }

                var possibleGoingStops = StaticData.ListStopOrdered
                    .Where(x => possibleGoingId.Contains(x.BusStopId)).Select(x => x.BusStop).ToList();
                var nearestStop = FindNearestStop(destination, possibleGoingStops);
                if (DistanceBetween(LatLng.Parse($"{nearestStop.Latitude},{nearestStop.Longitude}"), destination) < 0.5)
                {
                    var current = nearestStop.Id;
                    var list = new List<StopAccessibleViewModel>();
                    Stack<int> forwardStack = new Stack<int>();
                    forwardStack.Push(current);
                    while (forwardPrev[current] != initialId)
                    {
                        forwardStack.Push(forwardPrev[current]);
                        current = forwardPrev[current];
                    }
                    forwardStack.Push(initialId);
                    int prev = forwardStack.Pop();
                    while (forwardStack.Count != 0)
                    {
                        int stopC = forwardStack.Pop();
                        IEnumerable<StopAccessibleViewModel> stopAccessible =
                            Mapper.Map<List<StopAccessibleViewModel>>(StaticData.ListStopAccessibilities
                                .Where(x => x.InitialStopId == prev && x.DestStopId == stopC));
                        list.Add(Max(stopAccessible));
                        prev = stopC;
                    }

                    pathForContinueOnFoot = list;
                }
                //foreach (var i in possibleGoingId)
                //{
                //    if (DistanceBetween(Mapper.Map<BusStopViewModel>(Context.BusStops.Find(i)).LatLng, destination) <
                //        0.5)
                //    {
                //        int current = i;
                //        var list = new List<StopAccessibleViewModel>();
                //        Stack<int> forwardStack = new Stack<int>();
                //        forwardStack.Push(current);
                //        while (forwardPrev[current] != initialId)
                //        {
                //            forwardStack.Push(forwardPrev[current]);
                //            current = forwardPrev[current];
                //        }
                //        forwardStack.Push(initialId);
                //        int prev = forwardStack.Pop();
                //        while (forwardStack.Count != 0)
                //        {
                //            int stopC = forwardStack.Pop();
                //            IEnumerable<StopAccessibleViewModel> stopAccesibilities = 
                //                Mapper.Map<List<StopAccessibleViewModel>>(StaticData.ListStopAccessibilities
                //                .Where(x => x.InitialStopId == prev && x.DestStopId == stopC));
                //            list.Add(Max(stopAccesibilities));
                //            prev = stopC;
                //        }
                //        temp.Add(list);
                //    }
                //}
                var intersection = forwardMoved.Where(x => backwardMoved.Contains(x)).ToList();
                counter++;
                if (intersection.Count > 0)
                {
                    foreach (var item in intersection)
                    {
                        var list = new List<StopAccessibleViewModel>();
                        int current = item;
                        Stack<int> forwardStack = new Stack<int>();
                        Queue<int> backwardQueue = new Queue<int>();
                        while (forwardPrev[current] != initialId)
                        {
                            forwardStack.Push(forwardPrev[current]);
                            current = forwardPrev[current];
                        }
                        forwardStack.Push(initialId);
                        current = item;
                        while (backwardPrev[current] != destId)
                        {
                            backwardQueue.Enqueue(backwardPrev[current]);
                            current = backwardPrev[current];
                        }
                        backwardQueue.Enqueue(destId);
                        int prev = forwardStack.Pop();
                        IEnumerable<StopAccessibleViewModel> stopAccesibilities;
                        while (forwardStack.Count != 0)
                        {
                            int stopC = forwardStack.Pop();
                            stopAccesibilities = Mapper.Map<List<StopAccessibleViewModel>>(StaticData.ListStopAccessibilities.Where(x => x.InitialStopId == prev && x.DestStopId == stopC));
                            list.Add(Max(stopAccesibilities));
                            prev = stopC;
                        }
                        stopAccesibilities =
                            Mapper.Map<List<StopAccessibleViewModel>>(StaticData.ListStopAccessibilities.Where(x => x.InitialStopId == prev && x.DestStopId == item));
                        list.Add(Max(stopAccesibilities));
                        int queeStop = backwardQueue.Dequeue();
                        stopAccesibilities =
                            Mapper.Map<List<StopAccessibleViewModel>>(StaticData.ListStopAccessibilities.Where(x => x.InitialStopId == item && x.DestStopId == queeStop));
                        list.Add(Max(stopAccesibilities));
                        while (backwardQueue.Count != 0)
                        {
                            int stopC = backwardQueue.Dequeue();
                            stopAccesibilities = Mapper.Map<List<StopAccessibleViewModel>>(StaticData.ListStopAccessibilities.Where(x =>
                                x.InitialStopId == queeStop && x.DestStopId == stopC));
                            list.Add(Max(stopAccesibilities));
                            queeStop = stopC;
                        }
                        toReturn.Add(list);
                    }
                    break;
                }
            }

            if (pathForContinueOnFoot != null)
            {
                if (toReturn.Count == 0)
                {
                    return new List<List<StopAccessibleViewModel>>() { pathForContinueOnFoot };
                }
                return new List<List<StopAccessibleViewModel>>() { BestPath(toReturn), pathForContinueOnFoot };
            }

            if (toReturn.Count == 0)
            {
                return null;
            }
            return new List<List<StopAccessibleViewModel>>() { BestPath(toReturn) };
        }
        //TODO: Improve this function
        private List<StopAccessibleViewModel> BestPath(List<List<StopAccessibleViewModel>> paths)
        {
            if (paths == null)
            {
                throw new NullReferenceException();
            }
            if (paths.Count == 0)
            {
                throw new ArgumentException("List can't be empty");
            }
            var best = paths[0];
            var bestCount = paths[0].Count;
            for (var i = 1; i < paths.Count; i++)
            {
                if (paths[i].Count < bestCount)
                {
                    bestCount = paths[i].Count;
                    best = paths[i];
                }
            }

            return best;
        }


        private StopAccessibleViewModel Max(IEnumerable<StopAccessibleViewModel> stopAccesibilities)
        {
            var max = stopAccesibilities.First();
            int initialDelay = StaticData.ListStopOrdered.First(x => x.BusId == max.BusId && x.BusStopId == max.InitialStopId)
                .Delay;
            int destInationDelay = StaticData.ListStopOrdered
                .First(x => x.BusId == max.BusId && x.BusStopId == max.DestStopId).Delay;
            int secondsDelay = (initialDelay % 100) - (destInationDelay % 100);
            int minutesDelay = ((initialDelay % 10000) / 100) - ((destInationDelay % 1000) / 100);
            int hoursDelay = ((initialDelay % 1_000_000) / 100) - ((initialDelay % 1_000_000) / 100);
            if (secondsDelay < 0)
            {
                minutesDelay--;
                secondsDelay = 60 + secondsDelay;
            }

            if (minutesDelay < 0)
            {
                hoursDelay--;
                minutesDelay = 60 + minutesDelay;
            }

            int delay = int.Parse($"{hoursDelay}{minutesDelay}{secondsDelay}");
            foreach (var stopAccessibility in stopAccesibilities)
            {
                if (stopAccessibility.BusId == null)
                {
                    max = stopAccessibility;
                }

                int currentInitDelay = StaticData.ListStopOrdered.First(x =>
                    x.BusId == stopAccessibility.BusId && x.BusStopId == stopAccessibility.InitialStopId).Delay;
                int currentDestDelay = StaticData.ListStopOrdered.First(x =>
                    x.BusId == stopAccessibility.BusId && x.BusStopId == stopAccessibility.DestStopId).Delay;
                int currentSecondsDelay = (currentDestDelay % 100) - (currentInitDelay % 100);
                int currentMinutesDelay = ((currentDestDelay % 10000) / 100) - ((currentInitDelay % 1000) / 100);
                int currentHoursDelay = ((currentDestDelay % 1_000_000) / 100) - ((currentInitDelay % 1_000_000) / 100);
                if (currentSecondsDelay < 0)
                {
                    currentMinutesDelay--;
                    currentSecondsDelay = 60 + currentSecondsDelay;
                }

                if (currentMinutesDelay < 0)
                {
                    currentHoursDelay--;
                    currentMinutesDelay = 60 + currentMinutesDelay;
                }
                int currentDelay = int.Parse($"{currentHoursDelay}{currentMinutesDelay}{currentSecondsDelay}");
                if (currentDelay < delay)
                {
                    delay = currentDelay;
                    max = stopAccessibility;
                }
            }

            return max;
        }
    }
}