using System.ComponentModel.DataAnnotations.Schema;

namespace TravelActive.Models.Entities
{
    public class StopOrdered
    {
        public int Id { get; set; }
        [ForeignKey("BusStop")]
        public int BusStopId { get; set; }
        public BusStop BusStop { get; set; }
        public int BusId { get; set; }
        public Bus Bus { get; set; }
        public int Delay { get; set; }
    }
}