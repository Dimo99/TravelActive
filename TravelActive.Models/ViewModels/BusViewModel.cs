using Api.ION;
using AutoMapper;
using TravelActive.Common.Mapping;
using TravelActive.Common.Utilities;
using TravelActive.Models.Entities;

namespace TravelActive.Models.ViewModels
{
    public class BusViewModel : Resource,IHaveCustomMapping
    {
        public int Id { get; set; }
        public string BusName { get; set; }
        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<Bus, BusViewModel>()
                .ForMember(dest => dest.Self,
                    opt => opt.MapFrom(src => LinkGenerator.To(RouteNames.Bus, new {busId = src.Id})));
        }
    }
}