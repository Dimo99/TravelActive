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

        public Task<List<BusStopViewModel>> GetAllBusStops(SearchOptions<BusStopViewModel, BusStop> searchOptions)
        {
            IQueryable<BusStop> query = Context.BusStops;
            query = searchOptions.Apply(query);
            return query.ProjectTo<BusStopViewModel>().ToListAsync();
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
            await Context.SaveChangesAsync();
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
            await Context.SaveChangesAsync();
            return bus.Id;
        }
        
        public Task<List<BusStopViewModel>> GetBusStops(int id)
        {
            var busStops = Context.StopsOrdered.Include(x => x.BusStop).ProjectTo<BusStopViewModel>().ToListAsync();
            return busStops;
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