using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelActive.Models.Entities
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public User RequestedBy { get; set; }
        public User RequestedTo { get; set; }
        [ForeignKey("RequestedBy")]
        public string RequstedById { get; set; }
        [ForeignKey("RequestedTo")]
        public string RequestedToId { get; set; }

        public DateTime RequestTime { get; set; }
        public bool Accepted  { get; set; }
    }
    
}