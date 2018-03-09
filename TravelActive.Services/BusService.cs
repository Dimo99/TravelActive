using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.ION;
using Api.Query.Search;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TravelActive.Data;
using TravelActive.Models.BindingModels;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;

namespace TravelActive.Services
{
    public class BusService : Service
    {
        private IMapper mapper;
        public BusService(TravelActiveContext context, IMapper mapper) : base(context)
        {
            this.mapper = mapper;
        }


        private int DelaysAdd(int first, int second)
        {
            int minutesFirst = first % 100;
            int hoursFirst = first / 100;
            int minutesSecond = second % 100;
            int hoursSecond = second / 100;
            int finalMinutes = minutesFirst + minutesSecond;
            int finalHours = hoursFirst + hoursSecond;
            if (finalMinutes > 60)
            {
                finalHours++;
                finalMinutes = finalMinutes - 60;
            }

            if (finalHours >= 24)
            {
                finalHours -= 24;
            }

            return int.Parse($"{finalHours}{finalMinutes}");
        }

        private int DelaysSubstract(int first, int second)
        {
            int firstMinutes = first % 100;
            int firstHours = first / 100;
            int secondMinutes = second % 100;
            int secondHours = second / 100;
            int finalMinutes = secondMinutes - firstMinutes;
            int finalHours = secondHours - firstHours;
            if (finalMinutes < 0)
            {
                finalMinutes = 60 + finalMinutes;
            }

            if (finalHours < 0)
            {
                finalHours = 23 + finalHours;
            }

            return int.Parse($"{finalHours}{finalMinutes}");
        }
        public async Task<List<BusStopViewModel>> GetAllBusStopsAsync(SearchOptions<BusStopViewModel, BusStop> searchOptions)
        {
            IQueryable<BusStop> query = Context.BusStops;
            query = searchOptions.Apply(query);
            List<BusStop> busStops = await query.ToListAsync();
            List<BusStopViewModel> busStopView = mapper.Map<List<BusStop>, List<BusStopViewModel>>(busStops);
            List<List<PartialBusView>> list = new List<List<PartialBusView>>(busStopView.Count);
            foreach (var busStopViewModel in busStopView)
            {
                list.Add(new List<PartialBusView>());
            }
            for (int i = 0; i < busStops.Count; i++)
            {
                var buses = Context.StopsOrdered.Where(x => x.BusStopId == busStops[i].Id).Include(x => x.Bus);
                foreach (var bus in buses)
                {
                    DateTime now = DateTime.Now;
                    var busName = bus.Bus.BusName;
                    int delay = bus.Delay / 100;
                    var departureTimes = Context.DepartureTimes.Where(x => x.BusId == bus.BusId).OrderBy(x => x.Departuretime);
                    var stopTimes = departureTimes.Select(x => DelaysAdd(x.Departuretime / 100, delay));
                    int nowInt = int.Parse($"{now.Hour}{now.Minute}");
                    int nearest = stopTimes.FirstOrDefault(x => x > nowInt);
                    int sub = DelaysSubstract(nowInt, nearest);
                    if (nearest == 0)
                    {
                        list[i].Add(new PartialBusView()
                        {
                            BusName = busName,
                            Arival = "No information",
                            Delay = "No information"
                        });
                    }
                    else
                    {
                        list[i].Add(new PartialBusView()
                        {
                            BusName = busName,
                            Arival = $"{nearest / 100}:{nearest % 100}",
                            Delay = $"{(sub % 100) + (sub / 100) * 60} минути"
                        });
                    }
                }

                busStopView[i].Buses = list[i].ToArray();
            }

            return busStopView;
        }

        public void AddBusStop(BusStopBindingModel busStopBindingModel)
        {
            BusStop busStop = mapper.Map<BusStop>(busStopBindingModel);
            Context.BusStops.Add(busStop);
            Context.SaveChanges();
        }

        public ComplexBusViewModel GetBus(int id)
        {
            var busView = mapper.Map<ComplexBusViewModel>(Context.Busses.Find(id));
            return busView;
        }

        public Task<List<BusViewModel>> GetBuses()
        {
            return Context.Busses.ProjectTo<BusViewModel>().ToListAsync();
        }

        public Task<BusStopViewModel> GetBusStop(string name)
        {
            return Context.BusStops.ProjectTo<BusStopViewModel>().FirstOrDefaultAsync(x => x.StopName == name);
        }

        public async Task<int> CreateBus(BusBindingModel busBindingModel)
        {
            var bus = new Bus() { BusName = busBindingModel.BusName };
            bus = Context.Busses.Add(bus).Entity;
            int result = await Context.SaveChangesAsync();
            foreach (var busStop in busBindingModel.BusStops)
            {
                StopOrdered stopOrdered = new StopOrdered()
                {
                    BusId = bus.Id,
                    BusStopId = busStop.StopId,
                };
                stopOrdered.Delay = int.Parse(string.Join("", busStop.Delay.Split(':')));
                Context.StopsOrdered.Add(stopOrdered);
            }
            for (int i = 0; i < busBindingModel.BusStops.Length - 1; i++)
            {
                var c = busBindingModel.BusStops[i];
                for (int j = i + 1; j < busBindingModel.BusStops.Length; j++)
                {
                    var c1 = busBindingModel.BusStops[j];
                    StopAccessibility stopAccessibility = new StopAccessibility()
                    {
                        BusId = bus.Id,
                        InitialStopId = c.StopId,
                        DestStopId = c1.StopId
                    };
                    Context.StopsAccessibility.Add(stopAccessibility);
                }
            }
            DepartureTime departureTime = new DepartureTime();
            foreach (var time in busBindingModel.DepartureTimes)
            {
                departureTime.Departuretime = int.Parse(string.Join("", time.Split(':'))) * 100;
                departureTime.BusId = bus.Id;
                Context.DepartureTimes.Add(departureTime);
            }
            await Context.SaveChangesAsync();
            return bus.Id;
        }

        public async Task<List<BusStopViewModel>> GetBusStops(int id)
        {
            var busStops = await Context.StopsOrdered.Where(x=>x.BusId==id).Include(x => x.BusStop).ToListAsync();
            var busStopsViews = mapper.Map<List<BusStopViewModel>>(busStops);
            for (int i = 0; i < busStops.Count; i++)
            {
                int delay = busStops[i].Delay;
                if (delay < 60)
                {
                    busStopsViews[i].Delay = $"00:00:{delay:00}";
                    continue;
                }

                if (delay < 6000)
                {
                    busStopsViews[i].Delay = $"00:{(delay / 100):00}:{(delay % 100):00}";;
                    continue;
                }

                busStopsViews[i].Delay = $"{(delay / 10000):00}:{(delay / 100 % 100):00}:{(delay % 100):00}";
            }
            return busStopsViews;
        }

        public async Task<Collection<string>> GetBusDepartureTimes(int id)
        {
            var departureTimes = await Context.DepartureTimes
                .Where(d => d.BusId == id)
                .Select(x => $"{x.Departuretime.ToString().Substring(0, 2)}:{x.Departuretime.ToString().Substring(2)}")
                .ToArrayAsync();
            return new Collection<string>()
            {
                Value = departureTimes
            };
        }
    }
}