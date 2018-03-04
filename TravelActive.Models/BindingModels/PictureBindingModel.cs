using System.ComponentModel.DataAnnotations;

namespace TravelActive.Models.BindingModels
{
    public class PictureBindingModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "mediatype")]
        public string MediaType { get; set; }

        [Required]
        public string Value{ get; set; }
        [Required]
        public string Type { get; set; }
    }
}