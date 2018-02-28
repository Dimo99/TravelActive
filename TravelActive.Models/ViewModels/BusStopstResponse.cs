using Api.ION;

namespace TravelActive.Models.ViewModels
{
    public class BusStopstResponse : Collection<BusStopViewModel>
    {
        public Link BusStopSequence { get; set; }
        public Form BusStopForm { get; set; }
        public Link BusStopByName { get; set; }
        public Link DepartureTimes { get; set; }
        public Form BusStopsQueryForm { get; set; }
    }
}