using System.ComponentModel.DataAnnotations;
using Api.ION;

namespace TravelActive.Models.BindingModels
{
    public class LoginBindingModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [Secret]
        public string Password { get; set; }
    }
}