using System.ComponentModel.DataAnnotations;

namespace TravelActive.Models.BindingModels
{
    public class ForgotenPasswordBindingModel
    {
        [Required]
        public string Email { get; set; }
    }
}