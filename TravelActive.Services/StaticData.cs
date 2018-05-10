using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TravelActive.Data;
using TravelActive.Models.Entities;

namespace TravelActive.Services
{
    public static class StaticData
    {
        public static List<StopOrdered> ListStopOrdered;
        public static List<StopAccessibility> ListStopAccessibilities;

        public static void Initialize(TravelActiveContext context)
        {
            ListStopAccessibilities = context.StopsAccessibility.ToList();
            ListStopOrdered = context.StopsOrdered.Include(x => x.BusStop).ToList();
        }
    }
}