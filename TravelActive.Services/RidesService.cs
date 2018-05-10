using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelActive.Common.Extensions;
using TravelActive.Data;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;

namespace TravelActive.Services
{
    public class RidesService : Service
    {
        public RidesService(TravelActiveContext context) : base(context)
        {
        }

        public async Task AddRide(Ride ride, CancellationToken ct)
        {
            await Context.Rides.AddAsync(ride,ct);
            await Context.SaveChangesAsync(ct);
        }

        public async Task<RidesViewModelCollection> GetRide(string userId)
        {
            var user = await Context.Users.Include(x => x.Rides).FirstOrDefaultAsync(x => x.Id == userId);

            var userRides = user.Rides.ToArray();
            RidesViewModelCollection rides = MapRidesToRidesViewCollection(userRides);
            return rides;
        }

        private static RidesViewModelCollection MapRidesToRidesViewCollection(Ride[] userRides)
        {
            RidesViewModelCollection rides = new RidesViewModelCollection();
            rides.Value = Mapper.Map<RidesViewModel[]>(userRides);
            rides.NumberOfRides = userRides.Length;
            rides.NumberOfRidesLasWeek = userRides.Count(x => x.DateTime.IsInThisWeek());
            rides.NumberOfRidesLastMonth = userRides.Count(x => x.DateTime.IsInThisMonth());
            rides.NumberOfRidesLastYear = userRides.Count(x => x.DateTime.IsInThisYear());
            rides.NumberOfRidesToday = userRides.Count(x => x.DateTime.IsToday());
            return rides;
        }

        public async Task<RidesViewModelCollection> GetRides()
        {
            var rides = await Context.Rides.ToArrayAsync();
            return MapRidesToRidesViewCollection(rides);
        }
    }
}