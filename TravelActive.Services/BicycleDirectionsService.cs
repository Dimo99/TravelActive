using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class BicycleDirectionsService : DirectionsService
    {
        public BicycleDirectionsService(TravelActiveContext context, IMapper mapper) : base(context, mapper)
        {
        }
        private async Task<List<BicycleStopViewModel>> BicycleStops(string city)
        {
            return await Context.BicycleStops.Where(x => x.City.Name == city).ProjectTo<BicycleStopViewModel>().ToListAsync();
        }
        public async Task<BicycleStopViewModel> FindNEarestBicycleStop(LatLng initialLocation)
        {
            List<BicycleStopViewModel> bicycleStops = await BicycleStops("Бургас");
            return FindNearestStop(initialLocation, bicycleStops) as BicycleStopViewModel;
        }
        public async Task<Directions> GetDirectionsBicycle(LatLng start, LatLng end)
        {
            string url = $"{Constants.BicycleRouteUrl}{start.Longitude},{start.Latitude};{end.Longitude},{end.Latitude}"; ;
            return await GetDirections(url);
        }
        public async Task AddBicycleStop(BicycleStopBindingModel bicycleStop)
        {
            BicycleStop stop = mapper.Map<BicycleStop>(bicycleStop);
            City city = await Context.Cities.FirstOrDefaultAsync(x => x.Name == bicycleStop.CityName);
            if (city == null)
            {
                var result = await Context.Cities.AddAsync(new City() { Name = bicycleStop.CityName });
                city = result.Entity;
            }

            stop.City = city;
            await Context.BicycleStops.AddAsync(stop);
            await Context.SaveChangesAsync();
        }


    }
}