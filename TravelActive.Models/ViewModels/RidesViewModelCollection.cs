using Api.ION;

namespace TravelActive.Models.ViewModels
{
    public class RidesViewModelCollection : Collection<RidesViewModel>
    {
        public int NumberOfRides { get; set; }
        public int NumberOfRidesToday { get; set; }
        public int NumberOfRidesLasWeek { get; set; }
        public int NumberOfRidesLastMonth { get; set; }
        public int NumberOfRidesLastYear { get; set; }
    }
}