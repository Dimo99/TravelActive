﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TravelActive.Common.Mapping;
using TravelActive.Models.BindingModels;

namespace TravelActive.Models.Entities
{
    public class User : IdentityUser, IHaveCustomMapping
    {
        public Picture ProfilePicture { get; set; }
        [ForeignKey("ProfilePicture")]  
        public int? ProfilePictureId { get; set; }
        public List<BlockedTokens> BlockedTokens { get; set; } = new List<BlockedTokens>();
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Ride> Rides { get; set; } = new List<Ride>();
        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<RegisterForm, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}