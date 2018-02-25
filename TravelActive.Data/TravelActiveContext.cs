using TravelActive.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TravelActive.Data
{
    public class TravelActiveContext : IdentityDbContext<User>
    {
        public TravelActiveContext(DbContextOptions<TravelActiveContext> options) 
            : base(options)
        {
        }

        public DbSet<Friend> Friends { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<BlockedTokens> BlockedTokenses { get; set; }
        public DbSet<BicycleStop> BicycleStops { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<BusStop> BusStops { get; set; }
        public DbSet<Bus> Busses { get; set; }
        public DbSet<DepartureTime> DepartureTimes { get; set; }
        public DbSet<StopOrdered> StopsOrdered { get; set; }
        public DbSet<StopAccessibility> StopsAccessibility { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder
                .Entity<BlockedTokens>()
                .HasOne(b => b.User)
                .WithMany(u => u.BlockedTokens)
                .HasForeignKey(b => b.UserId);
            builder
                .Entity<FriendRequest>()
                .HasOne(fr => fr.RequestedTo)
                .WithMany(u => u.FriendRequests)
                .HasForeignKey(fr => fr.RequestedToId);
            builder
                .Entity<Friend>()
                .HasOne(f => f.FriendUser)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.FriendId);

        }
    }
}