using System.ComponentModel.DataAnnotations.Schema;
using TravelActive.Common.Mapping;
using TravelActive.Models.BindingModels;

namespace TravelActive.Models.Entities
{
    public class BicycleStop : IMapFrom<BicycleStopBindingModel>
    {
        public int Id { get; set; }
        public string StopName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; }
        public int CityId { get; set; }
    }
}