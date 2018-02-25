using Api.ION;
using AutoMapper;
using TravelActive.Common.Mapping;
using TravelActive.Common.Utilities;
using TravelActive.Models.Entities;

namespace TravelActive.Models.ViewModels
{
    public class UserViewModel : Resource, IHaveCustomMapping
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureUrl { get; set; }
        public decimal Rating { get; set; }
        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<User, UserViewModel>()
                .ForMember(uvm => uvm.Rating, opt => opt.MapFrom(u => u.Rating / 100m))
                .ForMember(uvm => uvm.Self, opt => opt.MapFrom(src => LinkGenerator.To(RouteNames.UsersMe, null)));
        }
    }
}