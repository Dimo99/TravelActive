using System.ComponentModel.DataAnnotations;

namespace TravelActive.Models.BindingModels
{
    public class BusBindingModel
    {
        [Required]
        [Display(Description = "Bus name.")]
        public string BusName { get; set; }
        [Required]
        [Display(Description = "Bus stops, with the daleys for each stop, ordered the way the bus passes by them.")]
        public BusBusStopBinding[] BusStops { get; set; }
        [Required]
        [Display(Description = "Departure times of the bus.")]
        public string[] DepartureTimes { get; set; }
    }

    public class BusBusStopBinding
    {
        public int StopId { get; set; }
        public string Delay { get; set; }

    }
}