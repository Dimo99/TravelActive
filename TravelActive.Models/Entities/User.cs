using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TravelActive.Common.Mapping;
using TravelActive.Models.Forms;

namespace TravelActive.Models.Entities
{
    public class User : IdentityUser, IHaveCustomMapping
    {
        public string PictureUrl { get; set; }
        public List<BlockedTokens> BlockedTokens { get; set; } = new List<BlockedTokens>();
        [Range(minimum: 0, maximum: 5)]
        public int Rating { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Friend> Friends { get; set; }
        public List<FriendRequest> FriendRequests { get; set; }
        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<RegisterForm, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}