using System.ComponentModel.DataAnnotations;

namespace TravelActive.Models.BindingModels
{
    public class BicycleStopBindingModel
    {
        [Required]
        public string StopName { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+\.[0-9]+$",ErrorMessage = "Longitude should be valid floating point number")]
        public string Longitude { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+\.[0-9]+$",ErrorMessage = "Latitude should be valid floating point number")]
        public string Latitude { get; set; }
        [Required]
        public string CityName { get; set; }
    }
}