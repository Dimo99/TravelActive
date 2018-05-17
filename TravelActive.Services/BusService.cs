using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.ION;
using Api.Query.Search;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TravelActive.Common.Utilities;
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
        public async Task<List<BusStopViewModel>> GetAllBusStopsAsync(SearchOptions<BusStopViewModel, BusStop> searchOptions)
        {
            IQueryable<BusStop> query = Context.BusStops;
            query = searchOptions.Apply(query);
            List<BusStop> busStops = await query.ToListAsync();
            List<BusStopViewModel> busStopView = mapper.Map<List<BusStop>, List<BusStopViewModel>>(busStops);
            return busStopView;
        }

        public void AddBusStop(BusStopBindingModel busStopBindingModel)
        {
            int cityId = 0;
            if (double.Parse(busStopBindingModel.Longitude) > 27)
            {
                cityId = 1;
            }
            else
            {
                cityId = 2;
            }
            BusStop busStop = mapper.Map<BusStop>(busStopBindingModel);
            busStop.CityId = cityId;
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

        public async Task<BusStopViewModel> GetBusStop(int id, DateTime now)
        {
            var busStop = await Context.BusStops.ProjectTo<BusStopViewModel>().FirstOrDefaultAsync(x => x.Id == id);
            var buses = await Context.StopsOrdered.Where(x => x.BusStopId == busStop.Id).Include(x => x.Bus).ToArrayAsync();
            busStop.Buses = new PartialBusView[buses.Length];
            for (int i = 0; i < buses.Length; i++)
            {
                var busName = buses[i].Bus.BusName;
                int delay = buses[i].Delay;
                var departureTimes = Context.DepartureTimes.Where(x => x.BusId == buses[i].BusId).Select(x => x.Departuretime);
                int smallest = -1;
                int nowInt = DelaysUtlility.ConvertDateTimeToInt(now);
                foreach (var departureTime in departureTimes)
                {
                    int addition = DelaysUtlility.DelayAddition(delay, departureTime);
                    if (addition > nowInt)
                    {
                        if (smallest == -1)
                        {
                            smallest = addition;
                        }
                        else
                        {
                            if (smallest > addition)
                            {
                                smallest = addition;
                            }
                        }
                    }
                }

                if (smallest == -1)
                {
                    busStop.Buses[i] = new PartialBusView()
                    {
                        BusName = busName,
                        Arival = "No information",
                        Delay = "No information"

                    };
                }
                else
                {
                    busStop.Buses[i] = new PartialBusView()
                    {
                        BusName = busName,
                        Arival = DelaysUtlility.DelayToStringWithoutSeconds(smallest),
                        Delay = DelaysUtlility.DelayToInformationString(DelaysUtlility.DelaysSubstract(smallest, nowInt))

                    };
                }
            }
            return busStop;
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
                stopOrdered.Delay = DelaysUtlility.ParseDelay(busStop.Delay, "00:00:00");
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
                departureTime.Departuretime = DelaysUtlility.ParseDelay(time, "00:00");
                departureTime.BusId = bus.Id;
                Context.DepartureTimes.Add(departureTime);
            }
            await Context.SaveChangesAsync();
            return bus.Id;
        }

        public async Task<List<BusStopViewModel>> GetBusStops(int id)
        {
            var busStops = await Context.StopsOrdered.Where(x => x.BusId == id).Include(x => x.BusStop).ToListAsync();
            var busStopsViews = mapper.Map<List<BusStopViewModel>>(busStops);
            return busStopsViews;
        }

        public async Task<Collection<string>> GetBusDepartureTimes(int id)
        {
            var departureTimes = await Context.DepartureTimes
                .Where(d => d.BusId == id)
                .OrderBy(x => x.Departuretime)
                .Select(x => DelaysUtlility.DelayToStringWithoutSeconds(x.Departuretime))
                .ToArrayAsync();
            return new Collection<string>()
            {
                Value = departureTimes
            };
        }
    }
}