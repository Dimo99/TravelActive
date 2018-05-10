using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelActive.Models.Entities
{
    public class Ride
    {
        public int Id { get; set; }
        public User User { get; set; }
        [ForeignKey("Bus")]
        public int BusId { get; set; }
        public Bus Bus { get; set; }
        public DateTime DateTime  { get; set; }
    }
}