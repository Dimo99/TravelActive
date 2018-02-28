using Api.ION;

namespace TravelActive.Models.ViewModels
{
    public class RootResponse : Resource
    {
        public Link Directions { get; set; }
        public Link Users { get; set; }
        public Link Tokens { get; set; }
        public Link Buses { get; set; }
        public Link BusStops { get; set; }
    }
}