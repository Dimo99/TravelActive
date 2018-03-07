using System.Collections.Generic;

namespace TravelActive.Models.ViewModels
{
    public class BusDirections
    {
        public List<SubBusDirections> Directions { get; set; } = new List<SubBusDirections>();
    }

    public class SubBusDirections
    {
        public string Polyline { get; set; }
        public decimal Distance { get; set; }
        public decimal Duration { get; set; }
        public string Method { get; set; }
        public string Bus { get; set; }
    }
}