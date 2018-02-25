using System.ComponentModel.DataAnnotations;

namespace TravelActive.Models.BindingModels
{
    public class CoordinatesBindingModel
    {
        [Required]
        [RegularExpression("^[0-9]+\\.[0-9]+\\,[0-9]+\\.[0-9]+$",ErrorMessage = "Invalid point signiture")]
        public string StartingPoint { get; set; }
        [Required]
        [RegularExpression("^[0-9]+\\.[0-9]+\\,[0-9]+\\.[0-9]+$",ErrorMessage = "Invalid point signiture")]
        public string DestinationPoint { get; set; }
    }
}