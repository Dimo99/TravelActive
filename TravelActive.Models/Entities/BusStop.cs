using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using TravelActive.Common.Mapping;
using TravelActive.Models.BindingModels;

namespace TravelActive.Models.Entities
{
    public class BusStop : IMapFrom<BusStopBindingModel>
    {
        public int Id { get; set; }
        public string StopName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public City City { get; set; }
        [ForeignKey("City")]
        public int? CityId { get; set; }
    }
}