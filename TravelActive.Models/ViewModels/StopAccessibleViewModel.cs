using TravelActive.Common.Mapping;
using TravelActive.Models.Entities;

namespace TravelActive.Models.ViewModels
{
    public class StopAccessibleViewModel : IMapFrom<StopAccessibility>
    {
        public int InitialStopId { get; set; }
        public int DestStopId { get; set; }
        public int? BusId { get; set; }
    }
}