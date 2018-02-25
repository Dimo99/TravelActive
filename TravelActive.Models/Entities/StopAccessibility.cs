using System.ComponentModel.DataAnnotations.Schema;

namespace TravelActive.Models.Entities
{
    public class StopAccessibility
    {
        public int Id { get; set; }
        [ForeignKey("InitialStop")]
        public int InitialStopId { get; set; }
        public BusStop InitialStop { get; set; }
        [ForeignKey("DestStop")]
        public int DestStopId { get; set; }
        public BusStop DestStop { get; set; }
        [ForeignKey("Bus")]
        public int? BusId { get; set; }
        public Bus Bus { get; set; }
    }
}