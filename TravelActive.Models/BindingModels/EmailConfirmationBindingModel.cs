using System.ComponentModel.DataAnnotations;

namespace TravelActive.Models.BindingModels
{
    public class EmailConfirmationBindingModel
    {
        [Required]
        public string Token { get; set; }
    }
}