using System.ComponentModel.DataAnnotations;

namespace TravelActive.Models.BindingModels
{
    public class BusStopBindingModel
    {
        [Required]
        [Display(Description = "The stop name.")]
        public string StopName { get; set; }
        [Required]
        [Display(Description = "The stop longitude.")]
        public string Longitude { get; set; }
        [Required]
        [Display(Description = "The stop latitude.")]
        public string Latitude { get; set; }
    }
}