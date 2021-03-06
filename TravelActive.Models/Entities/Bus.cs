﻿using System.Collections.Generic;

namespace TravelActive.Models.Entities
{
    public class Bus
    {
        public int Id { get; set; }
        public string BusName { get; set; }
        public ICollection<DepartureTime> BusStopTimes { get; set; } = new List<DepartureTime>();
        public List<Ride> Rides { get; set; } = new List<Ride>();
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ BusName.GetHashCode();
        }
    }
}