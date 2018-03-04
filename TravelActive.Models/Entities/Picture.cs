using TravelActive.Common.Mapping;
using TravelActive.Models.BindingModels;

namespace TravelActive.Models.Entities
{
    public class Picture : IMapFrom<PictureBindingModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MediaType { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

    }
}