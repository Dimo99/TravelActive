using System.ComponentModel.DataAnnotations;
using TravelActive.Common.Mapping;
using TravelActive.Models.Entities;

namespace TravelActive.Models.ViewModels
{
    public class PictureViewModel : IMapFrom<Picture>
    {
        public string Name { get; set; }
        [Display(Name = "mediatype")]
        public string MediaType { get; set; }

        public string Value { get; set; }
        public string Type { get; set; }
    }
}