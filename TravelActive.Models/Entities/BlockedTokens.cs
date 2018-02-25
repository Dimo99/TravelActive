using System;
using System.ComponentModel.DataAnnotations;

namespace TravelActive.Models.Entities
{
    public class BlockedTokens
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExparationDate { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}