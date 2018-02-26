using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TravelActive.Models.BindingModels
{
    public class BicycleStopBindingModel
    {
        [Required]
        [Description("Stop name")]
        public string StopName { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+\.[0-9]+$",ErrorMessage = "Longitude should be valid floating point number")]
        [Description("Stop longitude")]
        public string Longitude { get; set; }
        [Required]
        [Description("Stop latitude")]
        [RegularExpression(@"^[0-9]+\.[0-9]+$",ErrorMessage = "Latitude should be valid floating point number")]
        public string Latitude { get; set; }
        [Required]
        [Description("CityName")]
        public string CityName { get; set; }
    }
}