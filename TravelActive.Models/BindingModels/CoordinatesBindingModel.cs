using System.ComponentModel.DataAnnotations;
using Api.Query.CustomAttributes.Direction;
using TravelActive.Common.Utilities;

namespace TravelActive.Models.BindingModels
{
    public class CoordinatesBindingModel
    {
        [Location]
        [Required]
        [RegularExpression(Constants.CoordinatesRegex, ErrorMessage = "Invalid point signiture")]
        public string StartingPoint { get; set; }
        [Location]
        [Required]
        [RegularExpression(Constants.CoordinatesRegex, ErrorMessage = "Invalid point signiture")]
        public string DestinationPoint { get; set; }
    }
}