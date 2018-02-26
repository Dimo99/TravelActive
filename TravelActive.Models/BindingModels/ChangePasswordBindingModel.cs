using System.ComponentModel.DataAnnotations;
using Api.ION;

namespace TravelActive.Models.BindingModels
{
    public class ChangePasswordBindingModel
    {
        [Required]
        [Secret]
        public string OldPassword { get; set; }
        [Required]
        [Secret]
        public string NewPassword { get; set; }
    }
}