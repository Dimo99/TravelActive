using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TravelActive.Common.Extensions;
using TravelActive.Common.Utilities;
using TravelActive.Data;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;

namespace TravelActive.Services
{
    public class BusDirectionsService : DirectionsService
    {

        public BusDirectionsService(TravelActiveContext context, IMapper mapper) : base(context, mapper)
        {
        }
        private BusStopViewModel FindNearestBusStopWithRemove(LatLng coordinatesStartingPoint, List<BusStopViewModel> busStops)
        {
            BusStopViewModel nearestStop = FindNearestStop(coordinatesStartingPoint, busStops) as BusStopViewModel;
            busStops.Remove(nearestStop);
            return nearestStop;
        }

        private async Task<Directions> GetDirectionsCar(string waypoints)
        {
            string url = $"{Constants.CarRouteUrl}{waypoints}";
            return await GetDirections(url);
        }
        private async Task<List<BusStopViewModel>> BusStops()
        {
            return await Context.BusStops.ProjectTo<BusStopViewModel>().ToListAsync();
        }
        public async Task<List<Directions>> BusAlgorithm(Coordinates coordinates)
        {
            // maybe instead of two queries for buses we should make just one and use select from the list cause db queries cause overhead
            // maybe LinkedList will be faster cause of the Remove of elements but lets leave like that for now
            List<BusStopViewModel> busStops = await BusStops();
            BusStopViewModel firstToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
            BusStopViewModel secondToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
            BusStopViewModel thirdToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
            busStops.AddMany(firstToLocation, secondToLocation, thirdToLocation);
            BusStopViewModel firstToDestination = FindNearestBusStopWithRemove(coordinates.EndPoint, busStops);
            BusStopViewModel secondToDestination = FindNearestBusStopWithRemove(coordinates.EndPoint, busStops);
            BusStopViewModel thirdToDestination = FindNearestBusStopWithRemove(coordinates.EndPoint, busStops);
            busStops.AddMany(firstToDestination, secondToDestination, thirdToDestination);
            var paths = await GetBiDirectionalBFSAsync(firstToLocation.Id, firstToDestination.Id);
            var paths1 = await GetBiDirectionalBFSAsync(firstToLocation.Id, secondToDestination.Id);
            var paths2 = await GetBiDirectionalBFSAsync(firstToLocation.Id, thirdToDestination.Id);
            var paths3 = await GetBiDirectionalBFSAsync(secondToLocation.Id, firstToDestination.Id);
            var paths4 = await GetBiDirectionalBFSAsync(secondToLocation.Id, secondToDestination.Id);
            var paths5 = await GetBiDirectionalBFSAsync(secondToLocation.Id, thirdToDestination.Id);
            var paths6 = await GetBiDirectionalBFSAsync(thirdToLocation.Id, firstToDestination.Id);
            var paths7 = await GetBiDirectionalBFSAsync(thirdToLocation.Id, secondToDestination.Id);
            var paths8 = await GetBiDirectionalBFSAsync(thirdToLocation.Id, thirdToDestination.Id);
            List<Directions> directions = await GetDirectionsBus(coordinates, paths);
            List<Directions> directions2 = await GetDirectionsBus(coordinates, paths1);
            List<Directions> directions3 = await GetDirectionsBus(coordinates, paths2);
            List<Directions> directions4 = await GetDirectionsBus(coordinates, paths3);
            List<Directions> directions5 = await GetDirectionsBus(coordinates, paths4);
            List<Directions> directions6 = await GetDirectionsBus(coordinates, paths5);
            List<Directions> directions7 = await GetDirectionsBus(coordinates, paths6);
            List<Directions> directions8 = await GetDirectionsBus(coordinates, paths7);
            List<Directions> directions9 = await GetDirectionsBus(coordinates, paths8);
            return SortDirections(directions, directions2, directions3, directions4, directions5, directions6,
                directions7, directions8,
                directions9).Take(3).ToList();
        }

        private async Task<List<Directions>> GetDirectionsBus(Coordinates coordinates, List<List<StopAccessibleViewModel>> paths)
        {
            List<Directions> toReturn = new List<Directions>();
            foreach (var path in paths)
            {
                Directions sum = new Directions();
                var firstStop = path[0];
                Directions directionsToTheFirstStop = await GetDirectionsByFoot(coordinates.StartingPoint,
                    Context.BusStops.ProjectTo<BusStopViewModel>().First(x => x.Id == firstStop.InitialStopId).LatLng);
                sum += directionsToTheFirstStop;
                foreach (var item in path)
                {
                    string waypoints = GetWayPoints(item);
                    Directions directionsBetweenStops = await GetDirectionsCar(waypoints);
                    sum += directionsBetweenStops;
                }
                var lastStop = path[path.Count - 1];
                Directions directionsToDestination = await GetDirectionsByFoot(Context.BusStops
                    .ProjectTo<BusStopViewModel>().First(x => x.Id == lastStop.DestStopId).LatLng, coordinates.EndPoint);
                sum += directionsToDestination;
                toReturn.Add(sum);
            }

            return toReturn;
        }

        private string GetWayPoints(StopAccessibleViewModel item)
        {
            string waypoints = "";
            List<BusStop> busStops = Context.StopsOrdered.Where(x => x.BusId == item.BusId).OrderBy(x => x.Id).Select(x => x.BusStop).ToList();
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
        private async Task<List<List<StopAccessibleViewModel>>> GetBiDirectionalBFSAsync(int initialId, int destId)
        {
            var path =
                await Context.StopsAccessibility
                    .Where(s => s.InitialStopId == initialId && s.DestStopId == destId)
                    .ProjectTo<StopAccessibleViewModel>()
                    .ToListAsync();

            var toReturn = new List<List<StopAccessibleViewModel>>();
            if (path.Count != 0)
            {
                foreach (var item in path)
                {
                    toReturn.Add(new List<StopAccessibleViewModel>() { item });
                }

                return toReturn;
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
            while (initialStops.Count != 0 || destinationStops.Count != 0)
            {
                var currentInitialId = initialStops.Dequeue();
                var currentDestId = destinationStops.Dequeue();
                var possibleGoingId = Context.StopsAccessibility
                    .Where(s => s.InitialStopId == currentInitialId && !usedInitial.Contains(s.DestStopId)).Select(x => x.DestStopId).ToHashSet();
                var possibleCommingId = Context.StopsAccessibility
                    .Where(s => s.DestStopId == currentDestId && !usedDestination.Contains(s.InitialStopId)).Select(x => x.InitialStopId).ToHashSet();
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

                var intersection = forwardMoved.Where(x => backwardMoved.Contains(x)).ToList();
                if (intersection.Count > 0)
                {
                    foreach (var item in intersection)
                    {
                        var list = new List<StopAccessibleViewModel>();
                        int current = item;
                        Stack<int> forwardStack = new Stack<int>();
                        Queue<int> backwardQueue = new Queue<int>();
                        while (forwardPrev.ContainsKey(current))
                        {
                            forwardStack.Push(forwardPrev[current]);
                            current = forwardPrev[current];
                        }
                        current = item;
                        while (backwardPrev.ContainsKey(current))
                        {
                            backwardQueue.Enqueue(backwardPrev[current]);
                            current = backwardPrev[current];
                        }
                        int prev = forwardStack.Pop();
                        IEnumerable<StopAccessibility> stopAccesibilities;
                        while (forwardStack.Count != 0)
                        {
                            int stopC = forwardStack.Pop();
                            stopAccesibilities = Context.StopsAccessibility.Where(x => x.InitialStopId == prev && x.DestStopId == stopC);
                            list.Add(Max(stopAccesibilities));
                            prev = stopC;
                        }
                        stopAccesibilities =
                            Context.StopsAccessibility.Where(x => x.InitialStopId == prev && x.DestStopId == item);
                        list.Add(Max(stopAccesibilities));
                        int queeStop = backwardQueue.Dequeue();
                        stopAccesibilities =
                            Context.StopsAccessibility.Where(x => x.InitialStopId == item && x.DestStopId == queeStop);
                        list.Add(Max(stopAccesibilities));
                        while (backwardQueue.Count != 0)
                        {
                            int stopC = backwardQueue.Dequeue();
                            stopAccesibilities = Context.StopsAccessibility.Where(x =>
                                x.InitialStopId == queeStop && x.DestStopId == stopC);
                            list.Add(Max(stopAccesibilities));
                            queeStop = stopC;
                        }
                        toReturn.Add(list);
                    }
                    break;
                }
            }

            return toReturn;
        }

        private StopAccessibleViewModel Max(IEnumerable<StopAccessibility> stopAccesibilities)
        {
            //TODO:Add real evaluation algorithm
            StopAccessibility max = stopAccesibilities.First();
            foreach (var stopAccessibility in stopAccesibilities)
            {
                if (stopAccessibility.BusId == null)
                {
                    max = stopAccessibility;
                }
            }

            return mapper.Map<StopAccessibleViewModel>(max);
        }
    }
}