using Api.ION;

namespace TravelActive.Models.ViewModels
{
    public class BusesResponse : Collection<BusViewModel>
    {
        public Form BusForm { get; set; }
    }
}