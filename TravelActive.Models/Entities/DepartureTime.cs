using System.ComponentModel.DataAnnotations.Schema;

namespace TravelActive.Models.Entities
{
    public class DepartureTime
    {
        public int Id { get; set; }
        public int Departuretime { get; set; }
        [ForeignKey("Bus")]
        public int BusId { get; set; }
        public Bus Bus { get; set; }
    }
}