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
            string[] tokens = waypoints.Split(';');
            Directions sum = new Directions();
            //Making multiple requests for better OSRM quality
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                string waypoint1 = tokens[i];
                string waypoint2 = tokens[i + 1];
                sum += await GetDirections($"{Constants.CarRouteUrl}{waypoint1};{waypoint2}");
            }

            return sum;
        }
        private async Task<List<BusStopViewModel>> BusStops()
        {
            return await Context.BusStops.ProjectTo<BusStopViewModel>().ToListAsync();
        }
        public async Task<List<BusDirections>> BusAlgorithm(Coordinates coordinates)
        {
            List<BusStopViewModel> busStops = await BusStops();
            BusStopViewModel firstToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
            BusStopViewModel secondToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
            BusStopViewModel thirdToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
            BusStopViewModel foutrhToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
            BusStopViewModel fifthToLocation = FindNearestBusStopWithRemove(coordinates.StartingPoint, busStops);
            busStops.AddMany(firstToLocation, secondToLocation, thirdToLocation, foutrhToLocation, fifthToLocation);
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
            List<BusDirections> directions = await GetDirectionsBus(coordinates, paths);
            List<BusDirections> directions2 = await GetDirectionsBus(coordinates, paths1);
            List<BusDirections> directions3 = await GetDirectionsBus(coordinates, paths2);
            List<BusDirections> directions4 = await GetDirectionsBus(coordinates, paths3);
            List<BusDirections> directions5 = await GetDirectionsBus(coordinates, paths4);
            List<BusDirections> directions6 = await GetDirectionsBus(coordinates, paths5);
            List<BusDirections> directions7 = await GetDirectionsBus(coordinates, paths6);
            List<BusDirections> directions8 = await GetDirectionsBus(coordinates, paths7);
            List<BusDirections> directions9 = await GetDirectionsBus(coordinates, paths8);
            return SortDirections(directions, directions2, directions3, directions4, directions5, directions6,
                directions7, directions8,
                directions9).Take(3).ToList();
        }

        private async Task<List<BusDirections>> GetDirectionsBus(Coordinates coordinates, List<List<StopAccessibleViewModel>> paths)
        {
            List<BusDirections> busDirectionses = new List<BusDirections>();
            foreach (var path in paths)
            {
                BusDirections current = new BusDirections();
                var firstStop = path[0];
                Directions directionsToTheFirstStop = await GetDirectionsByFoot(coordinates.StartingPoint,
                    Context.BusStops.ProjectTo<BusStopViewModel>().First(x => x.Id == firstStop.InitialStopId).LatLng);
                var debug = new SubBusDirections()
                {
                    Method = "foot",
                    Polyline = directionsToTheFirstStop.Polylines.First(),
                    Distance = directionsToTheFirstStop.Distance,
                    Duration = directionsToTheFirstStop.Duration,
                };
                current.Directions.Add(debug);
                foreach (var item in path)
                {
                    string waypoints = GetWayPoints(item);
                    Directions directionsBetweenStops = await GetDirectionsCar(waypoints);
                    string polyline = "";
                    foreach (var s in directionsBetweenStops.Polylines)
                    {
                        polyline = polyline.PolylineAdd(s);
                    }
                    current.Directions.Add(new SubBusDirections()
                    {
                        Bus = GetBusName(item.BusId),
                        Method = "bus",
                        Distance = directionsBetweenStops.Distance,
                        Duration = directionsBetweenStops.Duration,
                        Polyline = polyline
                    });
                }
                var lastStop = path[path.Count - 1];
                Directions directionsToDestination = await GetDirectionsByFoot(Context.BusStops
                    .ProjectTo<BusStopViewModel>().First(x => x.Id == lastStop.DestStopId).LatLng, coordinates.EndPoint);
                current.Directions.Add(new SubBusDirections()
                {
                    Polyline = directionsToDestination.Polylines.First(),
                    Method = "foot",
                    Distance = directionsToDestination.Distance,
                    Duration = directionsToDestination.Duration
                });
                busDirectionses.Add(current);
            }

            return busDirectionses;
        }

        private string GetBusName(int? itemBusId)
        {
            return Context.Busses.Find(itemBusId).BusName;

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
            StopAccessibility max = stopAccesibilities.First();
            int initialDelay = Context.StopsOrdered.First(x => x.BusId == max.BusId && x.BusStopId == max.InitialStopId)
                .Delay;
            int destInationDelay = Context.StopsOrdered
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

                int currentInitDelay = Context.StopsOrdered.First(x =>
                    x.BusId == stopAccessibility.BusId && x.BusStopId == stopAccessibility.InitialStopId).Delay;
                int currentDestDelay = Context.StopsOrdered.First(x =>
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

            return mapper.Map<StopAccessibleViewModel>(max);
        }
    }
}