using System.Collections.Generic;

namespace TravelActive.Models
{
    public class Previous
    {
        public int StopId { get; set; }
        public List<int> Buses { get; set; } = new List<int>();
    }
}