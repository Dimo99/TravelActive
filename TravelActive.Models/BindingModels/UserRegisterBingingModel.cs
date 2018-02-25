using System.ComponentModel.DataAnnotations;

namespace TravelActive.Models.BindingModels
{
    public class UserRegisterBingingModel
    {
        
        
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]

        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be atleast {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}