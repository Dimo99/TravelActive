using System;
using Microsoft.AspNetCore.Identity;
using TravelActive.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TravelActive.Data
{
    public class TravelActiveContext : IdentityDbContext<User, UserRole, string>
    {
        public TravelActiveContext(DbContextOptions<TravelActiveContext> options)
            : base(options)
        {
        }
        public DbSet<BlockedTokens> BlockedTokenses { get; set; }
        public DbSet<BicycleStop> BicycleStops { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<BusStop> BusStops { get; set; }
        public DbSet<Bus> Busses { get; set; }
        public DbSet<DepartureTime> DepartureTimes { get; set; }
        public DbSet<StopOrdered> StopsOrdered { get; set; }
        public DbSet<StopAccessibility> StopsAccessibility { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder
                .Entity<BlockedTokens>()
                .HasOne(b => b.User)
                .WithMany(u => u.BlockedTokens)
                .HasForeignKey(b => b.UserId);
            builder
                .Entity<BusStop>()
                .HasAlternateKey(bs => bs.StopName);

        }
    }
}