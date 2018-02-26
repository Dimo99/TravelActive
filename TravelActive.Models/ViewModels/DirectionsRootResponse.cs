using Api.ION;

namespace TravelActive.Models.ViewModels
{
    public class DirectionsRootResponse : Resource
    {
        public Form BicycleQueryForm { get; set; }
        public Form BusQueryForm { get; set; }
        public Form BicycleStop { get; set; }

    }
}