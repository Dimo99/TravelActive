using System.Collections.Generic;
using TravelActive.Models.Entities;

namespace TravelActive.Models
{
    public class BusesWithStations
    {
        public string StartStop { get; set; }
        public string EndStop { get; set; }
        public List<Bus> Busses { get; set; }
    }
}