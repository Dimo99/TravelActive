using System.ComponentModel.DataAnnotations.Schema;

namespace TravelActive.Models.Entities
{
    public class Friend
    {
        public int Id { get; set; }
        public User FriendUser { get; set; }
        [ForeignKey("FriendUser")]
        public string FriendId { get; set; }
    }
}